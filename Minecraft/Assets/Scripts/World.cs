using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class World : MonoBehaviour {
    public int seed;
    
    public Transform player;
    public Vector3 spawnPosition;
    public Material material;
    public BlockType[] blockTypes;
    private Chunk[,] chunks = new Chunk[VoxelData.WorldSizeChunks, VoxelData.WorldSizeChunks];
    private List<ChunkCoord> activeChunks = new List<ChunkCoord>();

    private ChunkCoord playerLastChunkCoord;
    private ChunkCoord playerChunkCoord;
    void Start() {
        Random.InitState(seed);
        spawnPosition = new Vector3(VoxelData.WorldSizeChunks * VoxelData.ChunkWidth / 2f, VoxelData.ChunkHeight + 2, VoxelData.WorldSizeChunks * VoxelData.ChunkWidth / 2f);
        GenerateWorld();
        playerLastChunkCoord = GetChunkCoordFromVector3(player.position);
    }

    void Update() {
        playerChunkCoord = GetChunkCoordFromVector3(player.position);
        if (!playerChunkCoord.Equals(playerLastChunkCoord)) {
            CheckViewDistance();
        }
    }

    public void GenerateWorld() {
        for (int x = VoxelData.WorldSizeChunks / 2 - VoxelData.ViewDistanceInChunks; x < VoxelData.WorldSizeChunks / 2 + VoxelData.ViewDistanceInChunks; x++) {
            for (int z = VoxelData.WorldSizeChunks / 2 - VoxelData.ViewDistanceInChunks; z < VoxelData.WorldSizeChunks / 2 + VoxelData.ViewDistanceInChunks; z++) {
                CreateNewChunk(x, z);
            }
        }

        //将玩家置于地图中心位置
        player.position = spawnPosition;
    }

    ChunkCoord GetChunkCoordFromVector3(Vector3 pos) {
        int x = Mathf.FloorToInt(pos.x / VoxelData.ChunkWidth);
        int z = Mathf.FloorToInt(pos.z / VoxelData.ChunkWidth);
        return new ChunkCoord(x, z);
    }
    void CheckViewDistance() {
        ChunkCoord coord = GetChunkCoordFromVector3(player.position);
        print("do check view distance");
        playerLastChunkCoord = coord;
        List<ChunkCoord> previouslyActiveChunks = new List<ChunkCoord>(activeChunks);//复制激活的列表
        // ChunkCoord tempChunkCoord = new ChunkCoord(0, 0);
        for (int x = coord.x - VoxelData.ViewDistanceInChunks; x < coord.x + VoxelData.ViewDistanceInChunks; x++) {
            for (int z = coord.z - VoxelData.ViewDistanceInChunks; z < coord.z + VoxelData.ViewDistanceInChunks; z++) {
                if (isChunkInWorld(new ChunkCoord(x,z))) {
                    if (chunks[x,z] == null) {
                        CreateNewChunk(x,z);
                    }else if (!chunks[x, z].isActive) {
                        chunks[x, z].isActive = true;
                        //已经创建过的但是设为未激活的chunk重新添加大激活列表中
                        activeChunks.Add(new ChunkCoord(x,z));
                    }
                }

                for (int i = 0; i < previouslyActiveChunks.Count; i++) {
                    if (previouslyActiveChunks[i].Equals(new ChunkCoord(x, z))) {
                        previouslyActiveChunks.RemoveAt(i);
                    }
                }
            }
        }

        foreach (ChunkCoord c in previouslyActiveChunks) {
            chunks[c.x, c.z].isActive = false;
        }
    }

    //创建chunk
    void CreateNewChunk(int x, int z) {
        chunks[x, z] = new Chunk(new ChunkCoord(x, z), this);
        //创建的时候添加到激活列表中
        activeChunks.Add(new ChunkCoord(x,z));
    }

    //chunk是否在世界中
    bool isChunkInWorld(ChunkCoord coord) {
        if (coord.x > 0 && coord.x < VoxelData.WorldSizeChunks - 1 && coord.z > 0 && coord.z < VoxelData.WorldSizeChunks - 1) {
            return true;
        } else {
            return false;
        }
    }

    bool IsVoxelInWorld(Vector3 pos) {
        if (pos.x >= 0 && pos.x < VoxelData.WorldSizeInVoxels && pos.y >= 0 && pos.y < VoxelData.ChunkHeight && pos.z >= 0 && pos.z < VoxelData.WorldSizeInVoxels) {
            return true;
        } else {
            return false;
        }
    }

    public byte GetVoxel(Vector3 pos) {
        int yPos = Mathf.FloorToInt(pos.y);
        //世界之外air
        if (!IsVoxelInWorld(pos)) {
            return 0;
        }
        // 地层为基岩
        if (yPos == 0) {
            return 1;
        }

        int terrainHeight = Mathf.FloorToInt(VoxelData.ChunkHeight * Noise.Get2DPerlin(new Vector2(pos.x,pos.z),500,0.25f));
        // print((terrainHeight));
        if (yPos == terrainHeight) {
            return 3;//grass
        }  else if (yPos > terrainHeight) {
            return 0;
        } else {
            return 2;//grass
        }
    }
}

[System.Serializable]
public class BlockType {
    public string blockName;
    public bool isSolid;
    [Header("Texture Value")] public int backFaceTexture;
    public int frontFaceTexture;
    public int topFaceTexture;
    public int bottomFaceTexture;
    public int leftFaceTexture;

    public int rightFaceTexture;

    //back front top bottom left right
    public int GetTextureID(int faceIndex) {
        switch (faceIndex) {
            case 0: return backFaceTexture;
            case 1: return frontFaceTexture;
            case 2: return topFaceTexture;
            case 3: return bottomFaceTexture;
            case 4: return leftFaceTexture;
            case 5: return backFaceTexture;
            default:
                Debug.Log("Error in GetTextureID,invalid face index");
                return -1;
        }
    }
}