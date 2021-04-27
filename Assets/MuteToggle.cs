using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MuteToggle : MonoBehaviour
{
   
    public GameObject buttonOn, buttonOff;
    private bool muteToggeled;

    // Start is called before the first frame update
    void Start()
    {
        muteToggeled = AudioListener.volume == 0;

        if (muteToggeled)
        {
            buttonOn.SetActive(false);
            buttonOff.SetActive(true);
        }
        else
        {
            buttonOff.SetActive(false);
            buttonOn.SetActive(true);
        }
    }

    public void Toggle()
    {
        muteToggeled = !muteToggeled;
        if (muteToggeled)
        {
            buttonOn.SetActive(false);
            buttonOff.SetActive(true);
        }
        else
        {
            buttonOff.SetActive(false);
            buttonOn.SetActive(true);
        }
        MenuFunctions.Mute();
    }

}
