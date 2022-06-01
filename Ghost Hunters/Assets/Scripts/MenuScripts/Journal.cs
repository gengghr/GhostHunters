using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Journal : MonoBehaviour
{
    public GameObject JournalUI;
    public GameObject HUD;
    public static bool open = false;

    // Update is called once per frame
    void Update()
    {
        if (PauseMenu.started)
        {
            if (!PauseMenu.paused && Keyboard.current[Key.J].wasPressedThisFrame)
            {
                if (open)
                {
                    open = false;
                    JournalUI.SetActive(false);
                    HUD.SetActive(true);
                }
                else
                {
                    open = true;
                    JournalUI.SetActive(true);
                    HUD.SetActive(false);

                }
            }
        }
    }
}
