using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {
    public Material material;
    public BlockType[] blockTypes;

    void Start() {
        Chunk chunk1 = new Chunk(new ChunkCoord(0,0),this);
        Chunk chunk2 = new Chunk(new ChunkCoord(1,0),this);
    }

    public void GenerateWorld() {
        
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