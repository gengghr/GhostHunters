using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Handles the Detection Sphere. Detection Sphere is a placeholder item that should no longer be used.
public class DetectSphereInteractable : PlayerInteractable
{
  public override bool Interact(GameObject caller)
  {
    //Do Nothing
    return false;
  }

  public override bool Drop(GameObject caller)
  {
    //Do Nothing
    return false;
  }
}
