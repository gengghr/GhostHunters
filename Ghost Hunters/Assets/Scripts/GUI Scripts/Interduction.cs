using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Interduction : MonoBehaviour
{
  private GameObject[] textbox;
  // Start is called before the first frame update
  void Start()
  {
    //Cursor.lockState = CursorLockMode.None;
    textbox = GameObject.FindGameObjectsWithTag("Interductions");

  }

  // Update is called once per frame
  void Update()
  {
    if (Keyboard.current[Key.E].wasPressedThisFrame)
    okay();
  }
  public void okay()
  {
    foreach (var obj in textbox)
    {
      obj.SetActive(false);
    }
    Cursor.lockState = CursorLockMode.Locked;
    PauseMenu.started = true;
  }
}
