using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorUI : MonoBehaviour
{
    public GameObject doorUI;
    public static bool active;


    // door instruction UI control
    void Update()
    {
        if(active)
        {
            doorUI.SetActive(true);
        }
        else
        {
            doorUI.SetActive(false);
        }
    }
}
