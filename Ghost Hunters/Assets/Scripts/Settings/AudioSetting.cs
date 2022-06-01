using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioSetting : MonoBehaviour
{
    [SerializeField]
    Slider volume;

    // set and get volume
    void Start()
    {
        if(!PlayerPrefs.HasKey("musicVolume"))
        {
            PlayerPrefs.SetFloat("musicVolume", 1);
            Load();
        }
        else
        {
            Load();
        }
    }

    // change volume
    public void changeVolume()
    {
        AudioListener.volume = volume.value;
        Save();
    }

    // get volume
    private void Load()
    {
        volume.value = PlayerPrefs.GetFloat("musicVolume");
    }

    // save voulme to player
    private void Save()
    {
        PlayerPrefs.SetFloat("musicVolume", volume.value);
    }
}
