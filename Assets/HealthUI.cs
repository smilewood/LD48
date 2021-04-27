using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUI : MonoBehaviour
{
    public List<GameObject> Hearts;


    public delegate void HealthChangedDeligate(int ammount);
    public static HealthChangedDeligate OnHealthChanged;

    void Awake()
    {
        OnHealthChanged = SetHearts;
    }

    private void SetHearts(int count)
    {
        for( int i = 0; i < Hearts.Count; ++i)
        {
            Hearts[i].SetActive(i < count);
        }
    }
}
