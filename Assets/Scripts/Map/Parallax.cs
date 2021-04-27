using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    private float length, height;
    private Vector3 startPos;
    public float parallaxFactor;
    private GameObject cam;

    void Start()
    {
        startPos = transform.position;
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        length = renderer.bounds.size.x;
        height = renderer.bounds.size.y;
        cam = Camera.main.gameObject;
    }

    void Update()
    {
        Vector3 temp = cam.transform.position * (1 - parallaxFactor);
        float Xdistance = cam.transform.position.x * parallaxFactor;
        float Ydistance = cam.transform.position.y * parallaxFactor;

        Vector3 newPosition = new Vector3(startPos.x + Xdistance, startPos.y + Ydistance, transform.position.z);

        transform.position = newPosition;

        if (temp.x > startPos.x + (length / 2))
        {
            startPos += new Vector3(length, 0, 0);
        }
        else if (temp.x < startPos.x - (length / 2))
        {
            startPos -= new Vector3(length, 0, 0);
        }

        if (temp.y > startPos.y + (height / 2))
        {
            startPos += new Vector3(0, height, 0);
        }
        else if (temp.y < startPos.y - (height / 2))
        {
            startPos -= new Vector3(0, height, 0);
        }
    }

}
