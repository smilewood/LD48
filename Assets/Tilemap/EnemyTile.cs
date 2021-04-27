using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;
using System;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class EnemyTile : Tile
{

#if UNITY_EDITOR
    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        //Always show sprite when we are not running
        if (!Application.isPlaying)
        {
            tileData.sprite = sprite;
            tileData.color = Color.red;
        }
        else
        {
            //Remove sprite if we intend the sprite to be in-editor only
            //tileData.sprite = null;
            tileData.color = Color.green;
        }

        if (gameObject && tileData.gameObject == null)
        {
            tileData.gameObject = gameObject;
        }
    }
#endif


    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        if (Application.isPlaying)
        {
            tilemap.RefreshTile(position);
        }
    }

    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {
        RefreshTile(position, tilemap);
        return base.StartUp(position, tilemap, go);
    }

#if UNITY_EDITOR
    // The following is a helper that adds a menu item to create a GroundTile Asset
    [MenuItem("Assets/Create/Tiles/EnemyTile")]
    public static void CreateGroundTile()
    {

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (File.Exists(path))
        {
            path = Path.GetDirectoryName(path);
        }

        path = EditorUtility.SaveFilePanelInProject("Save Enemy Tile", "New Enemy Tile", "Asset", "Save Enemy Tile", path);
        if (path == "")
        {
            return;
        }

        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<EnemyTile>(), path);
    }
#endif
}
