using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {
    public Material material;
    public BlockType[] blockTypes;
    private Chunk[,] chunks = new Chunk[VoxelData.WorldSizeChunks, VoxelData.WorldSizeChunks];

    void Start() {
        GenerateWorld();
    }

    public void GenerateWorld() {
        for (int x = 0; x < VoxelData.WorldSizeChunks; x++) {
            for (int z = 0; z < VoxelData.WorldSizeChunks; z++) {
                CeateNewChunk(x, z);
            }
        }
    }
    //创建chunk
    void CeateNewChunk(int x, int z) {
        chunks[x, z] = new Chunk(new ChunkCoord(x, z), this);
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
        //air
        if (!IsVoxelInWorld(pos)) {
            return 4;
        }
        //底层
        if (pos.y < 1) {
            return 0;
        } else if (pos.y == VoxelData.ChunkHeight - 1) {
            //表层
            return 2; //草地
        } else {
            //中间层
            return 1;
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