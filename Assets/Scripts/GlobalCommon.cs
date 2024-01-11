using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using UnityEngine;

public class GlobalCommon : MonoBehaviour
{

    [SerializeField] public int mapX = 1500;
    [SerializeField] public int mapY = 1500;
    [SerializeField] public int zoomLevel = 17;
    [SerializeField] public float centerLong = -84.416501f;
    [SerializeField] public float centerLat = 33.741836f;
    [SerializeField] public float mpp = -1;
    [SerializeField] public float scaledMapX = -1;
    [SerializeField] public float scaledMapY = -1;
    [SerializeField] public float averageBuildingHeight = -1.0f;



    GlobalCommon()
    {
        float latRad = centerLat * Mathf.Deg2Rad;
        mpp = 156543.04f * Mathf.Cos(latRad) / Mathf.Pow(2, zoomLevel);

        scaledMapX = mapX * mpp;
        scaledMapY = mapY * mpp;

        Debug.Log("mpp: " + mpp + " scaledMapX: " + scaledMapX + " scaledMapY: " + scaledMapY);

    }

    public int[] GetAdjustedIndexXZ(int x, int z)
    {
        int[] adjustedXZ = new int[2];

        adjustedXZ[0] = x - (int)(mapX / 2);
        adjustedXZ[1] = (int)(mapY / 2) - z; // Y->Z conversion

        return adjustedXZ;
    }

    public float[] GetADjustedXZ(int x, int z)
    {
        int[] adjustedIndexXZ = GetAdjustedIndexXZ(x, z);
        float[] adjustedXZ = new float[2];

        adjustedXZ[0] = adjustedIndexXZ[0] * mpp;
        adjustedXZ[1] = adjustedIndexXZ[1] * mpp;

        // adjustedXZ[0] = adjustedIndexXZ[0];
        // adjustedXZ[1] = adjustedIndexXZ[1];

        return adjustedXZ;
    }


    // Singletone
    private static GlobalCommon _instance;
    public static GlobalCommon instance
    {
        get
        {
            if(_instance == null) _instance = GameObject.FindObjectOfType<GlobalCommon>();

            return _instance;
        }
    }

    private void Awake()
    {
        if(instance != this)
        {
            Destroy(gameObject);
        }
    }
}
