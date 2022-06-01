using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

/// <summary>
/// Meant to handle Bound Object Burning in Multiplayer
/// Based on NetFlammable
/// Author: Brandon Hullinger
/// Build: Final
/// </summary>
public class NetGhostBoundIgnite : NetFlammable
{
  public GameObject baseObject;
  public Material baseMat;
  public Material burnMat;

  protected virtual void Awake(){
    
  }

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
