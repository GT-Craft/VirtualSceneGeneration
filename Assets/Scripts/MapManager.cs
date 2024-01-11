using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    // Possible to generate the partial scene generation of the map with the following parameters
    [SerializeField] public int startX = 600, startZ = 1000, endX = 750, endZ = 1150;

    // Map tile includes the object information on the tile with its local and global coordinates to the map size.
    List<MapTile> mapTiles = new List<MapTile>();

    public MapTile GetMapTileByLocal(int x, int z)
    {
        int index = z * (endX - startX) + x;
        if (index < 0 || index >= mapTiles.Count)
        {
            Debug.Log("GetMapTileByLocal index out of range: " + index + " mapTiles.Count: " + mapTiles.Count);
            return null;
        }
        return mapTiles[index];
    }

    public MapTile GetMapTileByGlobal(int globalX, int globalZ)
    {
        int x = globalX - startX;
        int z = globalZ - startZ;
        return GetMapTileByLocal(x, z);
    }

    private static MapManager _instance;
    public static MapManager instance
    {
        get
        {
            if(_instance == null) _instance = FindObjectOfType<MapManager>();
            return _instance;
        }
    }

    private void Awake()
    {
        if(instance != this)
        {
            Destroy(gameObject);
        }

        if(mapTiles.Count == 0)
        {
            for(int z = startZ; z < endZ; z++)
            {
                for(int x = startX; x < endX; x++)
                {
                    MapTile mapTile = new MapTile();
                    mapTile.globalX = x;
                    mapTile.globalZ = z;
                    mapTile.x = x - startX;
                    mapTile.z = z - startZ;
                    mapTiles.Add(mapTile);
                }
            }
        }
    }
}
