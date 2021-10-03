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
    //存储三角形面片的数据
    public static readonly int[,] voxelTris = new int[6, 6] {
        {0,3,1,1,3,2},//front
        {5,6,4,4,6,7},//back
        {3,7,2,2,7,6},//top
        {1,5,0,0,5,4},//bottom
        {4,7,0,0,7,3},//left
        {1,2,5,5,2,6},//right
    };
}