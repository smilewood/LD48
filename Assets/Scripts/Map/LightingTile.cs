using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LightingTile : Tile
{
    public GameObject LightPrefab;

#if UNITY_EDITOR
    // The following is a helper that adds a menu item to create a GroundTile Asset
    [MenuItem("Assets/Create/LightTile")]
    public static void CreateLightTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Light Tile", "New Light Tile", "Asset", "Save Light Tile", "Assets");
        if (path == "")
        {
            return;
        }

        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<LightingTile>(), path);
    }
#endif
}
