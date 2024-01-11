using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalTypes : MonoBehaviour
{
}

public class MapTile
{
    public GameObject terrain;
    public GameObject building;
    public int globalX, globalZ;
    public int x, z;
}
