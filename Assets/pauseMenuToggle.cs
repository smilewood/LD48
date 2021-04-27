using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pauseMenuToggle : MonoBehaviour
{
    private bool paused = false;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused)
            {
                Unpause();
                paused = false;
            }
            else
            {
                MenuFunctions.PauseGame();
                MenuFunctions.Instance.ShowMenu("Pause");
                paused = true;
            }
        }
    }

    public void Unpause()
    {
        MenuFunctions.ResumeGame();
        MenuFunctions.Instance.HideMenu("Pause");
    }
}
