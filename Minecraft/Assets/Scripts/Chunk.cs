using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class Chunk : MonoBehaviour {
    public MeshRenderer meshRenderer; //网格渲染器
    public MeshFilter meshFilter; //网格过滤器

    int vertexIndex = 0; //顶点索引
    List<Vector3> vertices = new List<Vector3>(); //顶点列表 
    List<int> triangles = new List<int>(); //三角形列表
    List<Vector2> uvs = new List<Vector2>(); //uv列表

    byte[,,] voxelMap = new byte[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];
    public World world;

    void Awake() {
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
        world = GameObject.FindObjectOfType<World>();
    }

    void Start() {
        PopulateVoxelMap();
        CreateMeshData();
        CreateMesh();
    }

    //填充体素类型,0 grass草地,1 bedrock,2 stone岩石
    void PopulateVoxelMap() {
        for (int y = 0; y < VoxelData.ChunkHeight; y++) {
            for (int x = 0; x < VoxelData.ChunkWidth; x++) {
                for (int z = 0; z < VoxelData.ChunkWidth; z++) {
                    if (y < 1) {
                        voxelMap[x, y, z] = 0;
                    } else if (y == VoxelData.ChunkHeight - 1) {
                        voxelMap[x, y, z] = 3;
                    } else {
                        voxelMap[x, y, z] = 1;
                    }
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
        if (x < 0 || x > VoxelData.ChunkWidth - 1 || y < 0 || y > VoxelData.ChunkHeight - 1 || z < 0 ||
            z > VoxelData.ChunkWidth - 1) {
            return false;
        }

        return world.blockTypes[voxelMap[x, y, z]].isSolid;
    }

    //为chunk块设置体素数据,pos指定的渲染位置
    void AddVoxelDataToChunk(Vector3 pos) {
        for (int i = 0; i < VoxelData.voxelTris.GetLength(0); i++) {
            //pos + VoxelData.faceChecks[i] 得到该面法向量方向的位置,对该位置进行检测,有体素则该面不绘制
            if (!CheckVoxel(pos + VoxelData.faceChecks[i])) {
                for (int j = 0; j < VoxelData.voxelTris.GetLength(1); j++) {
                    //存储一个面的两个三角形顶点信息
                    vertices.Add(VoxelData.voxelVerts[VoxelData.voxelTris[i, j]] + pos);
                    // uvs.Add(VoxelData.voxelUvs[j]);
                }

                triangles.Add(vertexIndex);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 3);
                byte blockID = voxelMap[(int) pos.x, (int) pos.y, (int) pos.z];
                AddTexture(world.blockTypes[blockID].GetTextureID(i));
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

    void AddTexture(int textureID) {
        float y = textureID / VoxelData.TextureAtlasSizeInBlock;
        float x = textureID - (y * VoxelData.TextureAtlasSizeInBlock);

        x *= VoxelData.NormalizedBlockTextureSize;
        y *= VoxelData.NormalizedBlockTextureSize;

        y = 1f - y - VoxelData.NormalizedBlockTextureSize;
        // print(x+" "+y);
        uvs.Add(new Vector2(x, y));
        uvs.Add(new Vector2(x, y + VoxelData.NormalizedBlockTextureSize));
        uvs.Add(new Vector2(x + VoxelData.NormalizedBlockTextureSize, y));
        uvs.Add(new Vector2(x + VoxelData.NormalizedBlockTextureSize, y + VoxelData.NormalizedBlockTextureSize));
    }
}