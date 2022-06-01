using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// A fake camera for multiplayer.
/// Allows handling more stuff server side.
/// </summary>
public class NetPseudoCam : NetworkBehaviour
{
  public GameObject pseudoCamObject; //The object that represents a camera
  public Camera cam; //The camera it's emulating
  NetworkVariable<Vector3> netPos = new NetworkVariable<Vector3>();
  NetworkVariable<Vector3> netRot = new NetworkVariable<Vector3>();

  private void Awake()
  {
    netPos.OnValueChanged += SetNewPos;
    netRot.OnValueChanged += SetNewRot;
  }

  /// <summary>
  /// Updates the rotation when netRot updates.
  /// </summary>
  /// <param name="previousValue"></param>
  /// <param name="newValue"></param>
  private void SetNewRot(Vector3 previousValue, Vector3 newValue)
  {
    pseudoCamObject.transform.eulerAngles = newValue;
  }

  /// <summary>
  /// Updates the position when netPos updates.
  /// </summary>
  /// <param name="previousValue"></param>
  /// <param name="newValue"></param>
  private void SetNewPos(Vector3 previousValue, Vector3 newValue)
  {
    pseudoCamObject.transform.position = newValue;
  }

  // Start is called before the first frame update
  void Start()
  {
    if (cam == null)
    {
      cam = Camera.main;
    }
  }

  // Update is called once per frame
  void Update()
  {
    //In Single Player. Do Single Player stuff.
    
    if(NetworkManager.Singleton == null){
      Vector3 pos = cam.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, cam.nearClipPlane));
      Vector3 rot = cam.transform.eulerAngles;
      pseudoCamObject.transform.position = pos;
      pseudoCamObject.transform.eulerAngles = rot;
    }
    else{
      if(NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsServer){
        if(IsOwner){
          Vector3 pos = cam.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, cam.nearClipPlane));
          Vector3 rot = cam.transform.eulerAngles;
          updatePosRotServerRpc(pos, rot);
        }
      }
    }
    Debug.DrawRay(pseudoCamObject.transform.position, pseudoCamObject.transform.forward * 2.0f, Color.green);
  }

  /// <summary>
  /// Updates the pseudocam's position on the server to be passed to clients through network variable.
  /// </summary>
  /// <param name="pos"></param>
  /// <param name="rot"></param>
  [ServerRpc(RequireOwnership = false)]
  private void updatePosRotServerRpc(Vector3 pos, Vector3 rot)
  {
    netPos.Value = pos;
    netRot.Value = rot;
  }
}


