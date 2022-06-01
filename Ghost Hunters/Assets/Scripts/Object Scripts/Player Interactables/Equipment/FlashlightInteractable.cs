using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles Player Interaction with flashlight.
/// </summary>
public class FlashlightInteractable : PlayerInteractable
{
  public GameObject spotLight;

  public override bool Click(GameObject caller)
  {
    //Do Nothing
    return true;
  }

  /// <summary>
  /// Called Every Frame
  /// Handles flashlight orientation.
  /// </summary>
  protected override void Start()
  {
    hoverText.SetActive(false);
    spotLight.SetActive(false);
  }

  protected override void Update()
  {
    if(equippedTo != null){
      transform.rotation = equippedTo.GetComponent<PlayerInteraction>().playerCam.transform.rotation * Quaternion.Euler(equipRot);
    }
  }

  /// <summary>
  /// Turns flashlight on and off
  /// </summary>
  /// <param name="caller"></param>
  /// <returns></returns>
  public override bool Use(GameObject caller)
  {
    spotLight.SetActive(!spotLight.activeSelf);
    return true;
  }
}
