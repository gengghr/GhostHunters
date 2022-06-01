using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MusicController : MonoBehaviour
{
    //open setting for background music
  public AudioSource BackgroundMusic;
  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {
    //press P to play/ stop the music
    if (Keyboard.current[Key.P].wasPressedThisFrame)
    {
      if (BackgroundMusic.isPlaying)
      { BackgroundMusic.Stop(); }
      else
      {
        BackgroundMusic.Play();
      }
    }
  }
}
