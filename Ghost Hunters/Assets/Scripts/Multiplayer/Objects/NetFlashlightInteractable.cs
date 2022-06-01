using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Handles the Flashlight in Multiplayer
/// Look at NetPlayerInteractable for more information
/// Author: Brandon Hullinger
/// Build: Final
/// </summary>
public class NetFlashlightInteractable : NetPlayerInteractable
{
  public GameObject spotLight;
  protected NetworkVariable<bool> netSpotlightState = new NetworkVariable<bool>();

  /// <summary>
  /// Called when player clicks on item.
  /// </summary>
  /// <param name="caller">Player which called the action.</param>
  /// <returns></returns>
  public override bool Click(GameObject caller)
  {
    //Do Nothing
    return true;
  }

  /// <summary>
  /// Called when player releases the click on the object
  /// </summary>
  /// <param name="caller"></param>
  /// <returns></returns>
  public override bool ReleaseClick(GameObject caller)
  {
    //Do Nothing
    return true;
  }

  /// <summary>
  /// Called when the script first starts up
  /// </summary>
  protected override void Awake()
  {
    base.Awake();
    netSpotlightState.OnValueChanged += ChangeSpotlightState;
  }

  /// <summary>
  /// Updates the spotlight state using a Network Variable
  /// </summary>
  /// <param name="previousValue"></param>
  /// <param name="newValue"></param>
  private void ChangeSpotlightState(bool previousValue, bool newValue)
  {
    //Debug.Log("Changing Flashlight State");
    spotLight.SetActive(newValue);
  }

  /// <summary>
  /// Called on the object's first frame
  /// </summary>
  protected override void Start()
  {
    base.Start();
    spotLight.SetActive(false);
  }

  /// <summary>
  /// Called every frame.
  /// Handles angling the flashlight.
  /// </summary>
  protected override void Update()
  {
    ConnectionStart();
    if(NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer){
      if (equippedTo != baseParent)
      {
        transform.rotation = equippedTo.GetComponent<NetPlayerInteraction>().pseudoCam.transform.rotation * Quaternion.Euler(equipRot);
      }
    }
    
  }

  //Update Spotlight Server Side
  public override bool Use(GameObject caller)
  {
    bool newSpotlightState = !spotLight.activeSelf;
    if(NetworkManager.Singleton == null){
      spotLight.SetActive(newSpotlightState);
    }
    else{
      if(NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsClient){
        SetSpotlightStateServerRpc(newSpotlightState);
      }
    }
    return true;
  }

  /// <summary>
  /// ServerRpc to update the spotlight state
  /// </summary>
  /// <param name="newSpotlightState"></param>
  [ServerRpc(RequireOwnership = false)]
  private void SetSpotlightStateServerRpc(bool newSpotlightState)
  {
    netSpotlightState.Value = newSpotlightState;
  }

  /// <summary>
  /// Called when the flashlight first starts up as a server object.
  /// </summary>
  protected override void ConnectionStart(){
    if (NetworkManager.Singleton != null)
    {
      if (!Connected && NetworkManager.Singleton.IsServer)
      {
        netIsInteractable.Value = isInteractable;
        netEquippedTo.Value = baseParent.GetComponent<NetworkObject>();
        netSpotlightState.Value = false;
        Connected = true;
      }
    }
  }
}
