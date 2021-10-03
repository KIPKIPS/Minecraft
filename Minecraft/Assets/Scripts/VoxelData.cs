using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VoxelData {
    //存储体素的八个顶点信息
    public static readonly Vector3[] voxelVerts = new Vector3[8] {
        new Vector3(0f, 0f, 0f),
        new Vector3(1f, 0f, 0f),
        new Vector3(1f, 1f, 0f),
        new Vector3(0f, 1f, 0f),
        new Vector3(0f, 0f, 1f),
        new Vector3(1f, 0f, 1f),
        new Vector3(1f, 1f, 1f),
        new Vector3(0f, 1f, 1f),
    };

    public static readonly int[,] voxelTris = new int[1, 6] {
        {3,7,2,2,7,6}//top face
    };
}