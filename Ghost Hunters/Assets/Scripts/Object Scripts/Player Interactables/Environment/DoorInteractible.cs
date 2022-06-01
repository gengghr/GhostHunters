using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

/// <summary>
/// Handles opening and closing doors
/// Author: Brandon Hullinger
/// Build: Beta
/// </summary>
public class DoorInteractible : PlayerInteractable
{
  public Animator animator;
    [SerializeField]
    private InputDevice targetDevice;
    protected override void Start()
  {
    base.Start();
    animator = GetComponent<Animator>();
        List<InputDevice> inputDevices = new List<InputDevice>();
        InputDeviceCharacteristics rightController = InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller;
        InputDevices.GetDevicesWithCharacteristics(rightController, inputDevices);
        if (inputDevices.Count > 0)
        {
            targetDevice = inputDevices[0];
        }
    }

  public override bool Drop(GameObject caller)
  {
    return false;
  }

  public override bool Click(GameObject caller)
  {
    return false;
  }

  protected override void Update()
  {
        //Do Nothing
        targetDevice.TryGetFeatureValue(CommonUsages.gripButton, out bool pressed);
        if(pressed)
        {
            animator.SetBool("open", !animator.GetBool("open"));
        }
    }

  public override bool ReleaseClick(GameObject caller)
  {
    return false;
  }

  public override bool Interact(GameObject caller)
  {
    animator.SetBool("open", !animator.GetBool("open"));
    return true;
  }

  public override bool Hover(GameObject caller)
  {
    Camera cam = caller.GetComponent<PlayerInteraction>().playerCam;
    Vector3 itemWorldPos = transform.parent.position;
    hoverText.transform.LookAt(cam.transform);
    hoverText.transform.rotation = Quaternion.LookRotation(cam.transform.forward) * Quaternion.Euler(90, 0, 0);
    hoverText.transform.position = itemWorldPos + hoverTextPos + hoverText.transform.up * hoverTextClose * -1;
    hoverText.SetActive(true);
    return true;
  }

}
