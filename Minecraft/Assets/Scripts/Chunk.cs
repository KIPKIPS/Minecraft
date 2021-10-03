using System;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour {
    public MeshRenderer meshRenderer;//网格渲染器
    public MeshFilter meshFilter;//网格过滤器

    void Awake() {
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
    }

    void Start() {
        int vertexIndex = 0;//顶点索引
        List<Vector3> vertices = new List<Vector3>();//顶点列表 
        List<int> triangles = new List<int>();//三角形列表
        List<Vector2> uvs = new List<Vector2>();//uv列表
        for (int i = 0; i < VoxelData.voxelTris.GetLength(0); i++) {
            for (int j = 0; j < VoxelData.voxelTris.GetLength(1); j++) {
                //存储一个面的两个三角形顶点信息
                int triangleIndex = VoxelData.voxelTris[i, j];
                vertices.Add(VoxelData.voxelVerts[triangleIndex]);
                
                uvs.Add(Vector2.zero);
                triangles.Add(vertexIndex);
                vertexIndex++;
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();//从三角形和顶点重新计算网格的法线

        meshFilter.mesh = mesh;
    }
}