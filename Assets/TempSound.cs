using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempSound : MonoBehaviour
{
    private void Start()
    {
        AudioSource source = GetComponent<AudioSource>();
        source.Play();
        Destroy(this.gameObject, source.clip.length);
    }

}
