using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthController : MonoBehaviour
{
    public List<SpriteRenderer> Renderers;
    public GameObject Negative;
    public List<Sprite> Digits;
    public GameObject Target;

    public int Offset;
    public int Scale;

    void Update()
    {
        int value = (int)(Target.transform.position.y * Scale - Offset);

        if (value < 0)
        {
            Negative.SetActive(true);
        } else {
            Negative.SetActive(false);
        }

        value = Math.Abs(value);

        List<int> values = new List<int>();

        while (value > 0)
        {
            values.Add(value % 10);
            value = (int)(value / 10f);
        }

        for (int i = 0; i < Renderers.Count; i++)
        {
            int index = values.Count - (i + 1);
            if (index >= 0)
            {
                Renderers[i].enabled = true;
                Renderers[i].sprite = Digits[values[index]];
            } else {
                Renderers[i].enabled = false;
            }
        }
    }
}
