using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the Ghost Bound Object burning
/// Author: Brandon Hullinger
/// Build: Beta
/// </summary>
public class GhostBoundIgnite : Flammable
{
  public GameObject baseObject;
  public Material baseMat;
  public Material burnMat;

  public override bool Ignite()
  {
    if (burning) return false;
    baseObject.GetComponent<Renderer>().material = burnMat;
    burning = true;
    return true;
  }

  public override bool Douse()
  {
    baseObject.SetActive(true);
    baseObject.GetComponent<Renderer>().material = baseMat;
    burning = false;
    return false;
  }
}
