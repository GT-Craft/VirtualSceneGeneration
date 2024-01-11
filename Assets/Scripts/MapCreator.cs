using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MapCreator : MonoBehaviour
{
    // Terrain's elevation/x/z offset to align with the origin (0, 0, 0)
    [SerializeField] float terrainHeightOffset = 310.0f;
    [SerializeField] float xOffset = 0.0f;
    [SerializeField] float zOffset = 0.0f;

    // Building height -- currently set by a constant value, can be extrapolated with the exact building height
    [SerializeField] public float buildingHeight = 5.0f;
    // Scale factor to scale down the map
    [SerializeField] public int scaleFactor = 1;

    NavMeshSurface navMeshSurface;


    // Start is called before the first frame update
    void Start()
    {
        CreateTerrain();
        CreateBuilding();

        gameObject.AddComponent<NavMeshSurface>();
        navMeshSurface = FindObjectOfType<NavMeshSurface>();
        navMeshSurface.BuildNavMesh();
    }

    void CreateTerrain()
    {
        MapDataLoader mapDataLoader = FindObjectOfType<MapDataLoader>();
        float[,] elevationMap = mapDataLoader.elevationMap;
        int [,] roadSemantic = mapDataLoader.roadSemantic;

        for(int z = MapManager.instance.startZ; z < MapManager.instance.endZ; z+=scaleFactor)
        {
            for(int x = MapManager.instance.startX; x < MapManager.instance.endX; x+=scaleFactor)
            {
                float y = elevationMap[z, x] - terrainHeightOffset;

                float[] adjustedXZ = GlobalCommon.instance.GetADjustedXZ(x, z);
                if (xOffset == 0.0f) xOffset = adjustedXZ[0];
                if (zOffset == 0.0f) zOffset = adjustedXZ[1];

                MapTile mapTile = MapManager.instance.GetMapTileByGlobal(x, z);

                mapTile.terrain = GameObject.CreatePrimitive(PrimitiveType.Plane);
                mapTile.terrain.name = "Terrain_" + x + "_" + z;
                mapTile.terrain.transform.SetParent(MapManager.instance.transform);

                mapTile.terrain.transform.position = new Vector3(adjustedXZ[0]-xOffset, y, adjustedXZ[1]-zOffset);
                mapTile.terrain.transform.localScale = new Vector3(GlobalCommon.instance.mpp/10*scaleFactor, 1, GlobalCommon.instance.mpp/10*scaleFactor); // 1 meter scale with plane

                // mapTile.terrain.GetComponent<Renderer>().material.SetColor("_Color", mapDataLoader.mapTexture.GetPixel(x, (GlobalCommon.instance.mapY-z)-1));
                mapTile.terrain.GetComponent<Renderer>().material.color = mapDataLoader.mapTexture.GetPixel(x, (GlobalCommon.instance.mapY-z)-1);

                mapTile.terrain.AddComponent<NavMeshModifier>();
                if (roadSemantic[z, x] != 0)
                {
                    // mapTile.terrain.GetComponent<Renderer>().material.color = Color.black;
                    mapTile.terrain.GetComponent<NavMeshModifier>().overrideArea = true;
                    mapTile.terrain.GetComponent<NavMeshModifier>().area = 4;
                }
                else
                {
                    // mapTile.terrain.GetComponent<Renderer>().material.color = Color.white;
                    mapTile.terrain.GetComponent<NavMeshModifier>().overrideArea = true;
                    mapTile.terrain.GetComponent<NavMeshModifier>().area = 3;

                }
            }
        }
    }

    void CreateBuilding()
    {
        MapDataLoader mapDataLoader = FindObjectOfType<MapDataLoader>();
        float maxBuildingHeight = mapDataLoader.maxElevation - terrainHeightOffset + buildingHeight;
        int [,] buildingSemantic = mapDataLoader.buildingSemantic;

        for(int z = MapManager.instance.startZ; z < MapManager.instance.endZ; z+=scaleFactor)
        {
            for(int x = MapManager.instance.startX; x < MapManager.instance.endX; x+=scaleFactor)
            {
                if(buildingSemantic[z, x] == 0) continue;

                MapTile mapTile = MapManager.instance.GetMapTileByGlobal(x, z);
                float[] adjustedXZ = GlobalCommon.instance.GetADjustedXZ(x, z);
                if (xOffset == 0.0f) xOffset = adjustedXZ[0];
                if (zOffset == 0.0f) zOffset = adjustedXZ[1];
                float y = mapTile.terrain.transform.position.y;


                mapTile.building = GameObject.CreatePrimitive(PrimitiveType.Cube);
                mapTile.building.name = "Building_" + x + "_" + z;
                mapTile.building.transform.SetParent(MapManager.instance.transform);
                float buildingHeightFromTerrain = maxBuildingHeight - y;
                mapTile.building.transform.localScale = new Vector3(GlobalCommon.instance.mpp*scaleFactor, buildingHeightFromTerrain, GlobalCommon.instance.mpp*scaleFactor);

                mapTile.building.transform.position = new Vector3(adjustedXZ[0]-xOffset, y + buildingHeightFromTerrain/2, adjustedXZ[1]-zOffset);

                //mapTile.building.GetComponent<Renderer>().material.SetColor("_Color", mapDataLoader.mapTexture.GetPixel(x, (GlobalCommon.instance.mapY-z)-1));
                mapTile.building.GetComponent<Renderer>().material.color = mapDataLoader.mapTexture.GetPixel(x, (GlobalCommon.instance.mapY-z)-1);
            }
        }
    }
}
