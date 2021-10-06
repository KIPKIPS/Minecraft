using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VoxelData {
    public static readonly int ChunkWidth = 16; //块宽,voxel个数
    public static readonly int ChunkHeight = 128; //块高,voxel个数
    public static readonly int WorldSizeChunks = 100;
    
    public static readonly int ViewDistanceInChunks = 5;
    public static readonly int TextureAtlasSizeInBlock = 4;
    public static int WorldSizeInVoxels {
        get{ return WorldSizeChunks * ChunkWidth; }
    }

    public static float NormalizedBlockTextureSize {
        get{ return 1f / (float) TextureAtlasSizeInBlock; }
    }

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

    public static readonly Vector3[] faceChecks = new Vector3[6] {
        new Vector3(0f, 0f, -1f), //back
        new Vector3(0f, 0f, 1f), //front
        new Vector3(0f, 1f, 0f), //top
        new Vector3(0f, -1f, 0f), //bottom
        new Vector3(-1f, 0f, 0f), //left
        new Vector3(1f, 0f, 0f), //right
    };

    //存储三角形面片的数据
    public static readonly int[,] voxelTris = new int[6, 4] {
        //完整的面片数据
        //index 0 1 2 2 1 3
        // {0,3,1,1,3,2},//back
        // {5,6,4,4,6,7},//front
        // {3,7,2,2,7,6},//top
        // {1,5,0,0,5,4},//bottom
        // {4,7,0,0,7,3},//left
        // {1,2,5,5,2,6},//right
        //优化的去除重复数据
        {0, 3, 1, 2}, //back
        {5, 6, 4, 7}, //front
        {3, 7, 2, 6}, //top
        {1, 5, 0, 4}, //bottom
        {4, 7, 0, 3}, //left
        {1, 2, 5, 6}, //right
    };

    //uv
    public static readonly Vector2[] voxelUvs = new Vector2[4] {
        //完整数据
        // new Vector2(0, 0),
        // new Vector2(0, 1),
        // new Vector2(1, 0),
        // new Vector2(1, 0),
        // new Vector2(0, 1),
        // new Vector2(1, 1),
        //优化的去除重复数据
        new Vector2(0, 0),
        new Vector2(0, 1),
        new Vector2(1, 0),
        new Vector2(1, 1),
    };
}