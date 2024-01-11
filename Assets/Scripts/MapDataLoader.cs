using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class ElevationElem
{
    int x, y;
    float elevation;
}

public class MapDataLoader : MonoBehaviour
{
    // Elevation and semantic segmentation matrix
    public float [ , ] elevationMap;
    public int [ , ] roadSemantic;
    public int [ , ] buildingSemantic;

    // Map texture
    public Texture2D mapTexture;
    // Max elevation to cap the elevation map
    public float maxElevation = -1.0f;

    // Loading function for semantic segmentation matrix
    void LoadSemanticMask(int [,] mask, String path)
    {
        byte[] fileData = System.IO.File.ReadAllBytes(path);
        char[] charData = Encoding.ASCII.GetChars(fileData);

        int index = 0;
        for(int y = 0; y < GlobalCommon.instance.mapY; y++)
        {
            for(int x = 0; x < GlobalCommon.instance.mapX; x++)
            {
                index = y * GlobalCommon.instance.mapX + x;
                mask[y, x] = charData[index];
            }
        }
    }

    void Awake()
    {
        // Load semantic segmentation matrix
        roadSemantic = new int[GlobalCommon.instance.mapY, GlobalCommon.instance.mapX];
        buildingSemantic = new int[GlobalCommon.instance.mapY, GlobalCommon.instance.mapX];

        String roadSemanticPath = Application.dataPath + "/MapData/road_mask.bin";
        LoadSemanticMask(roadSemantic, roadSemanticPath);

        String buildingSemanticPath = Application.dataPath + "/MapData/building_mask.bin";
        LoadSemanticMask(buildingSemantic, buildingSemanticPath);

        // Load elevation matrix
        String elevationPath = Application.dataPath + "/MapData/elevation.bin";
        byte[] elevationData = System.IO.File.ReadAllBytes(elevationPath);
        float[] floatData = new float[elevationData.Length / 4];
        Buffer.BlockCopy(elevationData, 0, floatData, 0, elevationData.Length);
        Debug.Log("elevation len: " + floatData.Length);

        elevationMap = new float[GlobalCommon.instance.mapY, GlobalCommon.instance.mapX];
        int index = 0;
        for(int y = 0; y < GlobalCommon.instance.mapY; y++)
        {
            for(int x = 0; x < GlobalCommon.instance.mapX; x++)
            {
                index = y * GlobalCommon.instance.mapX + x;
                elevationMap[y, x] = floatData[index];
                if(elevationMap[y, x] > maxElevation) maxElevation = elevationMap[y, x];
            }
        }
        Debug.Log("maxElevation: " + maxElevation);

        // Load map texture
        String mapTexturePath = Application.dataPath + "/MapData/map_patch.png";
        byte[] mapTextureData = System.IO.File.ReadAllBytes(mapTexturePath);
        mapTexture = new Texture2D(2, 2);
        mapTexture.LoadImage(mapTextureData);

        Debug.Log("mapTexture len: " + mapTextureData.Length);
        Color32 pixel = mapTexture.GetPixel(0, 0);
        Debug.Log("pixel: " + pixel);
    }
}
