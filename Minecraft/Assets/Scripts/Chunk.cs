using System;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour {
    public MeshRenderer meshRenderer; //网格渲染器
    public MeshFilter meshFilter; //网格过滤器

    int vertexIndex = 0; //顶点索引
    List<Vector3> vertices = new List<Vector3>(); //顶点列表 
    List<int> triangles = new List<int>(); //三角形列表
    List<Vector2> uvs = new List<Vector2>(); //uv列表

    bool[,,] voxelMap = new bool[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];

    void Awake() {
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
    }

    void Start() {
        PopulateVoxelMap();
        CreateMeshData();
        CreateMesh();
    }

    //填充体素索引信息
    void PopulateVoxelMap() {
        for (int y = 0; y < VoxelData.ChunkHeight; y++) {
            for (int x = 0; x < VoxelData.ChunkWidth; x++) {
                for (int z = 0; z < VoxelData.ChunkWidth; z++) {
                    voxelMap[x, y, z] = true;
                }
            }
        }
    }

    void CreateMeshData() {
        Vector3 tempPos = new Vector3(0, 0, 0);
        for (int y = 0; y < VoxelData.ChunkHeight; y++) {
            for (int x = 0; x < VoxelData.ChunkWidth; x++) {
                for (int z = 0; z < VoxelData.ChunkWidth; z++) {
                    tempPos.x = x;
                    tempPos.y = y;
                    tempPos.z = z;
                    AddVoxelDataToChunk(tempPos);
                }
            }
        }
    }

    bool CheckVoxel(Vector3 pos) {
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);
        if (x < 0 || x > VoxelData.ChunkWidth - 1 || y < 0 || y > VoxelData.ChunkHeight - 1 || z < 0 || z > VoxelData.ChunkWidth - 1) {
            return false;
        }

        return voxelMap[x, y, z];
    }

    //为chunk块设置体素数据,pos指定的渲染位置
    void AddVoxelDataToChunk(Vector3 pos) {
        for (int i = 0; i < VoxelData.voxelTris.GetLength(0); i++) {
            //pos + VoxelData.faceChecks[i] 得到该面法向量方向的位置,对该位置进行检测,有体素则该面不绘制
            if (!CheckVoxel(pos + VoxelData.faceChecks[i])) {
                // for (int j = 0; j < VoxelData.voxelTris.GetLength(1); j++) {
                //     //存储一个面的两个三角形顶点信息
                //     int triangleIndex = VoxelData.voxelTris[i, j];
                //     vertices.Add(VoxelData.voxelVerts[triangleIndex] + pos);
                //
                //     uvs.Add(VoxelData.voxelUvs[j]);
                //     triangles.Add(vertexIndex);
                //     vertexIndex++;
                // }
                vertices.Add(VoxelData.voxelVerts[VoxelData.voxelTris[i, 0]] + pos);
                vertices.Add(VoxelData.voxelVerts[VoxelData.voxelTris[i, 1]] + pos);
                vertices.Add(VoxelData.voxelVerts[VoxelData.voxelTris[i, 2]] + pos);
                vertices.Add(VoxelData.voxelVerts[VoxelData.voxelTris[i, 3]] + pos);
                uvs.Add(VoxelData.voxelUvs[0]);
                uvs.Add(VoxelData.voxelUvs[1]);
                uvs.Add(VoxelData.voxelUvs[2]);
                uvs.Add(VoxelData.voxelUvs[3]);
                triangles.Add(vertexIndex);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 3);
                vertexIndex += 4;
            }
        }
    }

    void CreateMesh() {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals(); //从三角形和顶点重新计算网格的法线

        meshFilter.mesh = mesh;
    }
}