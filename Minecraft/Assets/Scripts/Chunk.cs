using System;
using System.Collections.Generic;
using UnityEngine;

public class Chunk {
    public ChunkCoord coord;
    GameObject chunkObject;
    MeshRenderer meshRenderer; //网格渲染器
    MeshFilter meshFilter; //网格过滤器

    int vertexIndex = 0; //顶点索引
    List<Vector3> vertices = new List<Vector3>(); //顶点列表 
    List<int> triangles = new List<int>(); //三角形列表
    List<Vector2> uvs = new List<Vector2>(); //uv列表

    byte[,,] voxelMap = new byte[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];
    public World world;

    public Chunk(ChunkCoord _coord, World _world) {
        chunkObject = new GameObject();
        coord = _coord;
        meshRenderer = chunkObject.AddComponent<MeshRenderer>();
        meshFilter = chunkObject.AddComponent<MeshFilter>();
        world = _world;
        meshRenderer.material = world.material;
        chunkObject.transform.SetParent(world.transform);
        chunkObject.transform.position = new Vector3(coord.x * VoxelData.ChunkWidth, 0, coord.z * VoxelData.ChunkWidth);
        chunkObject.name = "Chunk" + coord.x + "," + coord.z;
        PopulateVoxelMap();
        CreateMeshData();
        CreateMesh();
    }

    //填充体素类型
    void PopulateVoxelMap() {
        for (int y = 0; y < VoxelData.ChunkHeight; y++) {
            for (int x = 0; x < VoxelData.ChunkWidth; x++) {
                for (int z = 0; z < VoxelData.ChunkWidth; z++) {
                    //获取相对世界的体素块,需要加上chunk的相对位置position
                    voxelMap[x, y, z] = world.GetVoxel(new Vector3(x, y, z) + position);
                }
            }
        }
    }

    //创建mesh所需要的数据
    void CreateMeshData() {
        for (int y = 0; y < VoxelData.ChunkHeight; y++) {
            for (int x = 0; x < VoxelData.ChunkWidth; x++) {
                for (int z = 0; z < VoxelData.ChunkWidth; z++) {
                    if (world.blockTypes[voxelMap[x,y,z]].isSolid) {
                        AddVoxelDataToChunk(new Vector3(x,y,z));
                    }
                }
            }
        }
    }

    public bool isActive {
        get{ return chunkObject.activeSelf; }
        set{ chunkObject.SetActive(value); }
    }

    public Vector3 position {
        get{ return chunkObject.transform.position; }
    }

    //voxel是否在chunk中
    bool IsVoxelInChunk(int x, int y, int z) {
        if (x < 0 || x > VoxelData.ChunkWidth - 1 || y < 0 || y > VoxelData.ChunkHeight - 1 || z < 0 || z > VoxelData.ChunkWidth - 1) {
            return false;
        } else {
            return true;
        }
    }

    //检测voxel是否绘制该面
    bool CheckVoxel(Vector3 pos) {
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);
        //体素块不在chunk内
        if (!IsVoxelInChunk(x, y, z)) {
            //该体素位于相邻的chunk内,若是solid表示相邻的chunk内存在voxel相邻,也不进行绘制
            return world.blockTypes[world.GetVoxel(pos + position)].isSolid;
        } else {
            return world.blockTypes[voxelMap[x, y, z]].isSolid;
        }
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

    // 创建Mesh
    void CreateMesh() {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals(); //从三角形和顶点重新计算网格的法线

        meshFilter.mesh = mesh;
    }

    // 为mesh添加贴图
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

//块坐标
public class ChunkCoord {
    public int x;
    public int z;

    public ChunkCoord(int _x, int _z) {
        x = _x;
        z = _z;
    }

    public bool Equals(ChunkCoord other) {
        if (other == null) {
            return false;
        } else {
            return other.x == x && other.z == z;
        }
    }
}