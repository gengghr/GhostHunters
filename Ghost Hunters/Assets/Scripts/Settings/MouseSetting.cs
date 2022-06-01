using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseSetting : MonoBehaviour
{
    [SerializeField]
    Slider mouseSlider;

    public static float mouse = 1;

    void Start()
    {
        mouse = 1;
    }

    // change mouse speed
        public void change()
    {
        mouse = mouseSlider.value;
    }
}
