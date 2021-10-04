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

    void Awake() {
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
    }

    void Start() {
        Vector3 tempPos = new Vector3(0,0,0);
        for (int y = 0; y < VoxelData.ChunkHeight; y++) {
            for (int x = 0; x < VoxelData.ChunkWidth; x++) {
                for (int z = 0; x < VoxelData.ChunkWidth; z++) {
                    tempPos.x = x;
                    tempPos.y= y;
                    tempPos.z = z;
                    AddVoxelDataToChunk(tempPos);
                }
            }
        }
        CreateMesh();
    }

    //为chunk块设置体素数据,pos指定的渲染位置
    void AddVoxelDataToChunk(Vector3 pos) {
        for (int i = 0; i < VoxelData.voxelTris.GetLength(0); i++) {
            for (int j = 0; j < VoxelData.voxelTris.GetLength(1); j++) {
                //存储一个面的两个三角形顶点信息
                int triangleIndex = VoxelData.voxelTris[i, j];
                vertices.Add(VoxelData.voxelVerts[triangleIndex] + pos);

                uvs.Add(VoxelData.voxelUvs[j]);
                triangles.Add(vertexIndex);
                vertexIndex++;
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