using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public event Action<bool> OnTriggered;
    public bool LeftOfTrap;
    public AudioSource stepOnSource, stepOffSource;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            OnTriggered.Invoke(LeftOfTrap);
            stepOnSource.Play();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            stepOffSource.Play();
        }
    }

}
