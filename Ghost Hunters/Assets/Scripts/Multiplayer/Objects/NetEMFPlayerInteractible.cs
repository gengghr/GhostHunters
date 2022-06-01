using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using System;

/// <summary>
/// Handles the EMF reader for Multiplayer
/// Author: Brandon Hullinger
/// Build: Final
/// </summary>
public class NetEMFPlayerInteractible : NetPlayerInteractable
{
  
  public TextMeshPro emfText;
  private float readDelay;

  public float distToGhost;
  public float distToNearestBO;
  public float currentEMF;

  //Needs to be synchronized across all clients
  public List<GameObject> detecting = new List<GameObject>();


  // Start is called before the first frame update
  protected override void Start()
  {
    base.Start();
    if (detecting.Count == 0)
    {
      GameObject ghost = GameObject.FindGameObjectWithTag("Ghost");
      detecting.Add(ghost);
    }
    readDelay = 0.5f;
  }

  // Update is called once per frame
  protected override void Update()
  {
    //Make sure all EMF readers are reading correctly.
    if(detecting.Count <= 1 && NetworkManager.Singleton != null && !NetworkManager.IsServer){
      //Debug.Log("Trying to get Bound Objects");
      GetUpdatedDetectListServerRpc();
    }

    //Get and Display EMF
    if (readDelay > 0) readDelay -= Time.deltaTime;
    else
    {
      List<float> emfs = new List<float>();
      foreach (GameObject o in detecting)
      {
        if (o != null && o.activeSelf)
        {
          emfs.Add(o.GetComponent<NetEMFDetect>().getEMF(gameObject));
        }
      }
      float maxEmf = Mathf.Max(emfs.ToArray());
      currentEMF = maxEmf;
      emfText.text = maxEmf.ToString();
      //Debug.Log(emfText.text);
      if (maxEmf >= 4)
      {
        emfText.color = Color.red;
      }
      else if (maxEmf >= 2)
      {
        emfText.color = Color.yellow;
      }
      else
      {
        emfText.color = Color.green;
      }
      readDelay = 0.5f;
    }
  }

  /// <summary>
  /// Server RPC to get the detecting list from the server.
  /// </summary>
  [ServerRpc(RequireOwnership = false)]
  private void GetUpdatedDetectListServerRpc()
  {
    NetworkObjectReference[] detectingRefs = new NetworkObjectReference[detecting.Count];
    for(int i = 0; i < detectingRefs.Length; i++){
      detectingRefs[i] = detecting[i].GetComponent<NetworkObject>();
    }
    SendUpdateddDetectListClientRPC(detectingRefs);
  }

  /// <summary>
  /// Response from server to the client so the client can syncronize
  /// </summary>
  /// <param name="detectingRefs"></param>
  [ClientRpc]
  private void SendUpdateddDetectListClientRPC(NetworkObjectReference[] detectingRefs)
  {
    detecting = new List<GameObject>();
    foreach(NetworkObjectReference nor in detectingRefs){
      if(nor.TryGet(out NetworkObject netObj)){
        detecting.Add(netObj.gameObject);
      }
    }
  }

  public override bool Click(GameObject caller)
  {
    return false;
  }

  public override bool Hover(GameObject caller)
  {
    bool toReturn = base.Hover(caller);
    //Debug.Log("Hovering Over EMF");
    return toReturn;
  }
}
