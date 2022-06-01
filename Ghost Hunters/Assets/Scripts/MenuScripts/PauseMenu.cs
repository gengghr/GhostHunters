using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
  public static bool paused = false;
  static bool canStart = true;
  public static bool started = true;

  public GameObject PasueUI;
  public GameObject HUD;

    private void Start()
    {
        paused = false;
    }
    // Update is called once per frame
    void Update()
  {
    // game is started
    if (started)
    {
      if (Keyboard.current[Key.Escape].wasPressedThisFrame)
      {
        if (paused && canStart)
        {
          Resume();
        }
        else
        {
          Pause();
        }

      }
    }
  }
  void Pause()
  {
    HUD.SetActive(false);
    PasueUI.SetActive(true);
    paused = true;
  }
  public void Resume()
  {
    if (!Journal.open)
    {
      HUD.SetActive(true);
    }
    PasueUI.SetActive(false);
    paused = false;
  }
  public void MainMenu()
  {
    SceneManager.LoadScene(0);
  }
  public void Exit()
  {
    Debug.Log("Exit");
    //if (UnityEditor.EditorApplication.isPlaying)
    //    UnityEditor.EditorApplication.isPlaying = false;
    Application.Quit();
  }
  public void EnableStart()
  {
    canStart = true;
  }
  public void DisableStart()
  {
    canStart = false;
  }
  // cursor manage
  public void lockCursor(bool b)
  {
    if (b)
    {
      Cursor.lockState = CursorLockMode.Locked;
    }
    else
    {
      Cursor.lockState = CursorLockMode.None;
    }
  }
  public void setStarted(bool b)
  {
    started = b;
  }
}
