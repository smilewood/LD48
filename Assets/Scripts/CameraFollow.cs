using UnityEngine;
using System;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;
    private Vector3 pos;

    void Awake()
    {
        pos = transform.position;
    }

    private float cameraClamp(float value)
    {
        return (Mathf.Floor(value * 16.0f) / 16.0f) + (1f / 32f);
    }

    void FixedUpdate()
    {
        Vector3 desiredPos = target.position + offset;
        pos = Vector3.Lerp(pos, desiredPos, smoothSpeed);
        transform.position = new Vector3(cameraClamp(pos.x), cameraClamp(pos.y), cameraClamp(pos.z));
    }
}
