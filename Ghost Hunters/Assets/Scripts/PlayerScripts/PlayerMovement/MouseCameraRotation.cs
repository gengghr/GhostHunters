using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseCameraRotation : MonoBehaviour
{
  // horizontal rotation speed
  public float horizontalSpeed = 1f;
  //vertical rotation speed
  public float verticalSpeed = 1f;
  public float horizontalDeadSpace;
  public float verticalDeadSpace;
  private float xRotation = 0.0f;
  private float yRotation = 0.0f;
  public Camera cam;
  CharacterController characterController;

  // Start is called before the first frame update
  void Start()
  {
    Cursor.lockState = CursorLockMode.Locked;
    //cam = Camera.main;
    characterController = GetComponent<CharacterController>();
  }

  // Update is called once per frame
  void Update()
  {

    if (!PauseMenu.paused && !Journal.open)
    {
      Cursor.lockState = CursorLockMode.Locked;
      float mouseX = Mouse.current.delta.x.ReadValue();
      float mouseY = Mouse.current.delta.y.ReadValue();
      if (Mathf.Abs(mouseX) < horizontalDeadSpace)
      {
        mouseX = 0;
      }
      if (Mathf.Abs(mouseY) < verticalDeadSpace)
      {
        mouseY = 0;
      }

      mouseX *= horizontalSpeed;
      mouseY *= verticalSpeed;

      yRotation += mouseX;
      xRotation -= mouseY;
      xRotation = Mathf.Clamp(xRotation, -90, 90);

      cam.transform.eulerAngles = new Vector3(xRotation, yRotation, 0.0f);


      characterController.transform.eulerAngles = new Vector3(0.0f, yRotation, 0.0f);
    }
    else
    {
      Cursor.lockState = CursorLockMode.None;
    }
  }
}
