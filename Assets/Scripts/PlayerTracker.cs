using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PlayerTracker : MonoBehaviour
{
    public float CellSize = 32;

    private Vector2Int lastPos, curPos;
    private Vector2Int positive;

    private BoneMaster bones;
    // Start is called before the first frame update
    void Start()
    {
        bones = GameObject.Find("Grid").GetComponent<BoneMaster>();
        positive = new Vector2Int(gameObject.transform.position.x > 0 ? 1 : -1, gameObject.transform.position.y > 0 ? 1 : -1);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = this.transform.position;
        curPos = new Vector2Int(((int)(pos.x / CellSize))+ (pos.x > 0 ? 1 : 0), ((int)(pos.y / CellSize) + (pos.y > 0 ? 1 : 0)));
        if(lastPos != curPos)
        {
            bones.CheckUpdatedPlayerPosition(this.transform.position, this.gameObject);
        }

        positive.x = gameObject.transform.position.x > 0 ? 1 : -1;
        positive.y = gameObject.transform.position.y > 0 ? 1 : -1;
        lastPos = curPos;
    }
}
