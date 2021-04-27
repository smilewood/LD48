using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundCameraFollower : MonoBehaviour
{
    private static SoundCameraFollower instance;

    private void Awake()
    {
        if(instance is null)
        {
            instance = this;
        }
        else
        {
            DestroyImmediate(this.gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);
        SceneManager.sceneLoaded += OnLoadLevel;
        pos = transform.position;
    }

    private void OnLoadLevel(Scene scene, LoadSceneMode mode)
    {
        target = Camera.main.gameObject.transform;
    }

    public Transform target;
    public float smoothSpeed = 0.001f;
    public Vector3 offset = new Vector3(0, 0, -10);
    private Vector3 pos;


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
