using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BoneItializer : MonoBehaviour
{
    public Vector2 position;
    // Start is called before the first frame update
    public void OnBoneCreate(Vector2 bonePos, bool verticalBone)
    {
        position = bonePos;
        foreach (Transform t in this.transform)
        {
            if(t.gameObject.GetComponent<INeedsinitialization>() is INeedsinitialization child)
            {
                child.Init(bonePos, verticalBone);
            }
        }
    }

    public void OnPlayerInBone(GameObject player)
    {
        foreach (Transform t in this.transform)
        {
            if (t.gameObject.GetComponent<IPlayerInBone>() is IPlayerInBone child)
            {
                child.PlayerInBone(player);
            }
        }
    }
}
