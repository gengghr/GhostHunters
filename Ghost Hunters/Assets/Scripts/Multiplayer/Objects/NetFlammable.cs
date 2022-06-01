using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Handles objects that have burning states in multiplayer
/// Primarily uswed with bound objects
/// Author: Brandon Hullinger
/// Build: Final
/// </summary>
public class NetFlammable : NetworkBehaviour
{

  public GameObject flameParticleSystem;
  public GameObject flameLight;

  //Should be consistent across all clients
  public bool burning; //Handled with ClientRpc instead of Network Variable
  public float burnTime = 10f; //Handled with ClientRpc instead of Network Variable

  //Network Vars


  // Start is called before the first frame update
  protected virtual void Start()
  {
    Douse();
  }

  // Update is called once per frame
  protected virtual void Update()
  {
    if(burning){
      burnTime -= Time.deltaTime;
      if (burnTime < 0)
      {
        gameObject.SetActive(false);
      }
    }
  }

  /// <summary>
  /// Sets the object on fire
  /// </summary>
  /// <returns></returns>
  public virtual bool Ignite(){
    if (burning) return false;
    if (NetworkManager.Singleton == null)
    {
      flameParticleSystem.SetActive(true);
      flameLight.SetActive(true);
      burning = true;
      return true;
    }
    else if (NetworkManager.Singleton.IsServer)
    {
      flameParticleSystem.SetActive(true);
      flameLight.SetActive(true);
      burning = true;
      SetBurningClientRpc(true);
      return true;
    }
    return false;
  }

  /// <summary>
  /// Passes that the object is on fire to the clients
  /// </summary>
  /// <param name="v"></param>
  [ClientRpc]
  private void SetBurningClientRpc(bool v)
  {
    flameLight.SetActive(v);
    flameParticleSystem.SetActive(v);
    burning = v;
  }

  /// <summary>
  /// Douses the object
  /// </summary>
  /// <returns></returns>
  public virtual bool Douse(){
    if (!burning) return false;
    if (NetworkManager.Singleton == null)
    {
      flameParticleSystem.SetActive(false);
      flameLight.SetActive(false);
      burning = false;
      return true;
    }
    else if (NetworkManager.Singleton.IsServer)
    {
      flameParticleSystem.SetActive(false);
      flameLight.SetActive(false);
      burning = false;
      SetBurningClientRpc(false);
      return true;
    }
    return false;
  }
}
