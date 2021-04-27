using UnityEngine;
using UnityEngine.U2D;
using System.Collections;
using UnityEngine.Tilemaps;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class FloorTile : Tile
{
    public SpriteAtlas SpriteBase;

    public Sprite[] sprites;
    private Sprite[] Sprites
    {
        get
        {
            if(sprites is null)
            {
                sprites = new Sprite[SpriteBase.spriteCount];
                SpriteBase.GetSprites(sprites);
            }
            return sprites;
        }
    }


    public Sprite m_Preview;
    // This refreshes itself and other FloorTiles that are orthogonally and diagonally adjacent
    public override void RefreshTile(Vector3Int location, ITilemap tilemap)
    {
        for (int yd = -1; yd <= 1; yd++)
        {
            for (int xd = -1; xd <= 1; xd++)
            {
                Vector3Int position = new Vector3Int(location.x + xd, location.y + yd, location.z);
                if (HasFloorTile(tilemap, position))
                {
                    tilemap.RefreshTile(position);
                }
            }
        }
    }


    [Flags]
    private enum Neighbors
    {
        N = 1,
        E = 2,
        S = 4,
        W = 8,
        NE = 16,
        NW = 32,
        SE = 64,
        SW = 128
    }

    // This determines which sprite is used based on the FloorTiles that are adjacent to it and rotates it to fit the other tiles.
    // As the rotation is determined by the FloorTile, the TileFlags.OverrideTransform is set for the tile.
    public override void GetTileData(Vector3Int location, ITilemap tilemap, ref TileData tileData)
    {
        
        Neighbors neighbors = 0;

        neighbors |= (Neighbors)(HasFloorTile(tilemap, location + new Vector3Int(0, 1, 0)) ? 1 : 0);
        neighbors |= (Neighbors)(HasFloorTile(tilemap, location + new Vector3Int(1, 0, 0)) ? 2 : 0);
        neighbors |= (Neighbors)(HasFloorTile(tilemap, location + new Vector3Int(0, -1, 0)) ? 4 : 0);
        neighbors |= (Neighbors)(HasFloorTile(tilemap, location + new Vector3Int(-1, 0, 0)) ? 8 : 0);
        if((int)neighbors != 0)
        {
            neighbors |= (Neighbors)(HasFloorTile(tilemap, location + new Vector3Int(1, 1, 0)) ? 16 : 0);
            neighbors |= (Neighbors)(HasFloorTile(tilemap, location + new Vector3Int(-1, 1, 0)) ? 32 : 0);
            neighbors |= (Neighbors)(HasFloorTile(tilemap, location + new Vector3Int(1, -1, 0)) ? 64 : 0);
            neighbors |= (Neighbors)(HasFloorTile(tilemap, location + new Vector3Int(-1, -1, 0)) ? 128 : 0);
        }

        int index = GetIndex(neighbors);
        if(index == -1)
        {
            tileData.sprite = m_Preview;
        }
        else if (index < Sprites.Length)
        {
            tileData.sprite = Sprites[index];
            tileData.color = Color.white;
            var m = tileData.transform;
            tileData.transform = m;
            tileData.flags = TileFlags.LockTransform;
            tileData.colliderType = ColliderType.None;
        }
        else
        {
            Debug.LogWarning("Not enough sprites in RoadTile instance");
        }
    }
    // This determines if the Tile at the position is the same RoadTile.
    private bool HasFloorTile(ITilemap tilemap, Vector3Int position)
    {
        return !(tilemap.GetTile<FloorTile>(position) is null);
    }
    // The following determines which sprite to use based on the number of adjacent RoadTiles\\

    private readonly int[] maskIndexes = new int[]
    {
        70, 198, 140, 4, 191, 127, 175, 143, 87, 255, 173, 5, 239, 223, 31, 47, 19, 61, 41, 1, 251, 2, 10, 8, 0,
        253, 15, 247, -1, -1, 254, -1, -1, 23, 45, 142, 78, -1, -1, -1, -1, 71, 141, 43, 27, -1, -1, -1, -1
    };

    private int GetIndex(Neighbors mask)
    {
        return Array.IndexOf(maskIndexes, (int)mask);
    }
    // The following determines which rotation to use based on the positions of adjacent RoadTiles
    private Quaternion GetRotation(byte mask)
    {
        //switch (mask)
        //{
        //   case 9:
        //   case 10:
        //   case 7:
        //   case 2:
        //   case 8:
        //   return Quaternion.Euler(0f, 0f, -90f);
        //   case 3:
        //   case 14:
        //   return Quaternion.Euler(0f, 0f, -180f);
        //   case 6:
        //   case 13:
        //   return Quaternion.Euler(0f, 0f, -270f);
        //}
        return Quaternion.Euler(0f, 0f, 0f);
    }
#if UNITY_EDITOR
    // The following is a helper that adds a menu item to create a GroundTile Asset
    [MenuItem("Assets/Create/FLoorTile")]
    public static void CreateGroundTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Ground Tile", "New Ground Tile", "Asset", "Save Ground Tile", "Assets");
        if (path == "")
        {
            return;
        }

        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<FloorTile>(), path);
    }
#endif
}
