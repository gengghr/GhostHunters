using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

//Provides the default implementation of an interactible item class in Multiplayer
//Handles the items side of player-item interaction.
public class NetPlayerInteractable : NetworkBehaviour
{
  //Variables that only matter client side.
  public Vector3 equipLoc = new Vector3(0f, 0f, 0f);
  public Vector3 equipRot = new Vector3(0f, 0f, 0f);
  public float dragDist = 0.25f;
  protected bool drag = false;
  public GameObject hoverText;
  public Vector3 hoverTextPos;
  public float hoverTextClose;
  public GameObject baseParent;
  protected GameObject pseudoCamCurrent = null;

  public static List<string> intangibleTags = new List<string>() { "Item", "BoundItem", "ProtectionCircle", "ProtectionCircleCandleSlot" };

  public static List<GameObject> intangibleGameObjects = new List<GameObject>();

  protected bool Connected = false;

  //Variables that must be updated among all clients.
  public bool isInteractable = true;
  [SerializeField]
  public GameObject equippedTo;

  //Network Variables.
  protected NetworkVariable<bool> netIsInteractable = new NetworkVariable<bool>();
  protected NetworkVariable<NetworkObjectReference> netEquippedTo = new NetworkVariable<NetworkObjectReference>();

  //Called when player hovers reticule over object.
  //Returns True if action succeeded. False otherwise.
  //Hover is no different for SP/MP
  public virtual bool Hover(GameObject caller)
  {
    if (!isInteractable)
    {
      return false;
    }
    GameObject pseudoCam = caller.GetComponent<NetPlayerInteraction>().pseudoCam;
    Vector3 itemWorldPos = transform.position;
    hoverText.transform.LookAt(pseudoCam.transform);
    hoverText.transform.rotation = Quaternion.LookRotation(pseudoCam.transform.forward) * Quaternion.Euler(90, 0, 0);
    hoverText.transform.position = itemWorldPos + hoverTextPos + hoverText.transform.up * hoverTextClose * -1;
    hoverText.SetActive(true);
    return true;
  }

  //Called when player stops hovering reticule over object.
  //Returns True if action succeeded. False otherwise.
  //No difference between SP/MP.
  public virtual bool StopHover(GameObject caller)
  {
    hoverText.SetActive(false);
    return true;
  }

  //Called when player clicks on object
  //Base Case, item is picked up and carried. Not Equipped.
  //Returns True if action succeeded. False otherwise.
  //MP needs to update netcar isInteractable.
  //Update rigidbody server side
  public virtual bool Click(GameObject caller)
  {
    if (!isInteractable)
    {
      return false;
    }
    //gameObject.GetComponent<Rigidbody>().detectCollisions = false;
    clickServerRpc(caller.GetComponent<NetworkObject>());
    return true;
  }

  /// <summary>
  /// Sends RPC to server for object clicking
  /// </summary>
  /// <param name="networkObject"></param>
  [ServerRpc(RequireOwnership = false)]
  private void clickServerRpc(NetworkObjectReference networkObject)
  {
    if(networkObject.TryGet(out NetworkObject netCaller)){
      GameObject caller = netCaller.gameObject;
      pseudoCamCurrent = caller.GetComponent<NetPlayerInteraction>().pseudoCam;
      drag = true;
      isInteractable = false;
      gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
      gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
      GetComponent<Rigidbody>().isKinematic = true;
      GetComponent<Rigidbody>().detectCollisions = false;
      GetComponent<Rigidbody>().useGravity = false;
      updateIsInteractableServerRpc(isInteractable);
      updateRigidBodiesClientRpc(false);
    }
  }

  /// <summary>
  /// Returns Client RPC to set Rigid Body paramters.
  /// </summary>
  /// <param name="v"></param>
  [ClientRpc]
  private void updateRigidBodiesClientRpc(bool v)
  {
    GetComponent<Rigidbody>().isKinematic = !v;
    GetComponent<Rigidbody>().detectCollisions = v;
    GetComponent<Rigidbody>().useGravity = v;
  }

  /// <summary>
  /// Updates the Interactable status of the object
  /// </summary>
  /// <param name="toSet"></param>
  [ServerRpc(RequireOwnership = false)]
  public void updateIsInteractableServerRpc(bool toSet)
  {
    netIsInteractable.Value = toSet;
  }

  //Called when player releases clicking on an object
  //Base Case, stop carrying clicked object
  //Returns True if action succeeded. False otherwise.
  //Needs to update netvar isInteractable
  //Update rigidbody server side
  public virtual bool ReleaseClick(GameObject caller)
  {
    releaseClickServerRpc();
    return true;
  }

  /// <summary>
  /// Server Rpc for when releasing clicking.
  /// </summary>
  [ServerRpc(RequireOwnership = false)]
  private void releaseClickServerRpc()
  {
    pseudoCamCurrent = null;
    drag = false;
    isInteractable = true;
    gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
    gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    GetComponent<Rigidbody>().isKinematic = false;
    GetComponent<Rigidbody>().detectCollisions = true;
    GetComponent<Rigidbody>().useGravity = true;
    updateIsInteractableServerRpc(isInteractable);
    updateRigidBodiesClientRpc(true);
  }

  //Called when player presses 'Interact" key (Default E)
  //Base Case, item is equipped if there is an empty equipment slot.
  //Returns True if action succeeded. False otherwise.
  //Needs to update netvar isInteractable
  //Needs to update netvar equippedTo
  //Needs to update parent and rigidbody server side
  //Position set Server side
  public virtual bool Interact(GameObject caller)
  {
    if (isInteractable && caller.GetComponent<NetPlayerInteraction>().CanEquip())
    {
      isInteractable = false;
      caller.GetComponent<NetPlayerInteraction>().Equipped.Add(gameObject);
      equippedTo = caller;
      //Zero Position Before Parent Set

      if (NetworkManager.Singleton == null){
        GetComponent<Rigidbody>().isKinematic = true;
        gameObject.GetComponent<Rigidbody>().detectCollisions = false;
        gameObject.GetComponent<Rigidbody>().useGravity = false;
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        transform.eulerAngles = new Vector3(0, 0, 0);
        transform.position = new Vector3(0, 0, 0);
        transform.SetParent(caller.transform);
        transform.localPosition = equipLoc;
        transform.localEulerAngles = equipRot;
      }
      else if(NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsClient){
        updateIsInteractableServerRpc(isInteractable);
        updateEquippedToServerRpc(equippedTo.GetComponent<NetworkObject>());
        repositionServerRpc(Vector3.zero, Vector3.zero);
        updateParentServerRpc(equippedTo.GetComponent<NetworkObject>());
        repositionServerRpc(equipLoc, equipRot);
        updateRigidBodyServerRpc(false);
        zeroRigidBodyVelicityServerRpc();
      }

      return true;
    }
    return false;
  }

  /// <summary>
  /// Server RPC for repositioning this object
  /// </summary>
  /// <param name="pos"></param>
  /// <param name="rot"></param>
  [ServerRpc(RequireOwnership = false)]
  public void repositionServerRpc(Vector3 pos, Vector3 rot)
  {
    transform.localPosition = pos;
    transform.localEulerAngles = rot;
  }

  /// <summary>
  /// Server Rpc for repositioning on a global transform
  /// </summary>
  /// <param name="pos"></param>
  /// <param name="rot"></param>
  [ServerRpc(RequireOwnership = false)]
  public void repositionGlobalServerRpc(Vector3 pos, Vector3 rot){
    transform.position = pos;
    transform.eulerAngles = rot;
  }

  /// <summary>
  /// Server Rpc for zeroing out rigid body velocities
  /// </summary>
  [ServerRpc(RequireOwnership = false)]
  protected void zeroRigidBodyVelicityServerRpc()
  {
    gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
    gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
  }

  /// <summary>
  /// Server Rpc for changing which object this one is equipped to
  /// </summary>
  /// <param name="equippedToRef"></param>
  [ServerRpc(RequireOwnership = false)]
  protected void updateEquippedToServerRpc(NetworkObjectReference equippedToRef)
  {
    netEquippedTo.Value = equippedToRef;
  }

  /// <summary>
  /// Server Rpc to update object parenting. Used primarily for equipping and dropping.
  /// </summary>
  /// <param name="parentRef"></param>
  [ServerRpc(RequireOwnership = false)]
  protected void updateParentServerRpc(NetworkObjectReference parentRef)
  {
    if(parentRef.TryGet(out NetworkObject parentObj)){
      transform.SetParent(parentObj.transform);
    }
    else{
      transform.SetParent(baseParent.transform);
    }
  }

  //Called when player uses item while equpped
  //Returns True if action succeeded. False otherwise.
  //Needs to update isInteractable and EquippedTo if does anything with equipment.
  public virtual bool Use(GameObject caller)
  {
    //Implement on an item by item basis.
    return true;
  }

  //Called when player places item in the world
  //Returns True if action succeeded. False otherwise.
  //Needs to update isInteractable and equippedTo if does anything with equipment.
  public virtual bool Place(GameObject caller)
  {
    //Called on item by item basis.
    //Most should not have a 'place' attribute.
    return true;
  }

  //Called when player drops the item without placing
  //Base: Let go of equipped item and let it drop.
  //Returns True if action succeeded. False otherwise.
  //Needs to update netvars isInteractable and equippedTo
  //Needs to update rigidboy and parent server side.
  public virtual bool Drop(GameObject caller)
  {
    //Debug.Log("Dropping");
    if(equippedTo == baseParent){
      return false;
    }
    caller.GetComponent<NetPlayerInteraction>().Equipped.RemoveAt(caller.GetComponent<NetPlayerInteraction>().inHand);
    isInteractable = true;
    equippedTo = baseParent;
    if (NetworkManager.Singleton == null)
    {
      gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
      gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
      GetComponent<Rigidbody>().isKinematic = false;
      GetComponent<Rigidbody>().detectCollisions = true;
      GetComponent<Rigidbody>().useGravity = true;
      transform.SetParent(baseParent.transform);
    }
    else if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsClient)
    {
      updateIsInteractableServerRpc(isInteractable);
      updateEquippedToServerRpc(equippedTo.GetComponent<NetworkObject>());
      updateParentServerRpc(baseParent.GetComponent<NetworkObject>());
      updateRigidBodyServerRpc(true);
      zeroRigidBodyVelicityServerRpc();
    }
    return true;
  }

  /// <summary>
  /// Server Rpc to update rigid body paramters.
  /// </summary>
  /// <param name="v"></param>
  [ServerRpc(RequireOwnership = false)]
  protected void updateRigidBodyServerRpc(bool v)
  {
    GetComponent<Rigidbody>().isKinematic = !v;
    GetComponent<Rigidbody>().useGravity = v;
    GetComponent<Rigidbody>().detectCollisions = v;
  }

  /// <summary>
  /// Called when the script first activates
  /// </summary>
  protected virtual void Awake()
  {
    netIsInteractable.OnValueChanged += IsInteractableChanged;
    netEquippedTo.OnValueChanged += EquippedToChanged;
  }

  /// <summary>
  /// Called when the object this is equipped to changes.
  /// </summary>
  /// <param name="previousValue"></param>
  /// <param name="newValue"></param>
  protected void EquippedToChanged(NetworkObjectReference previousValue, NetworkObjectReference newValue)
  {
    if(newValue.TryGet(out NetworkObject newObj)){
      equippedTo = newObj.gameObject;
    }
    else{
      equippedTo = baseParent;
    }
  }

  /// <summary>
  /// Called when netIsInteractable changes.
  /// Updates isInteractable
  /// </summary>
  /// <param name="previousValue"></param>
  /// <param name="newValue"></param>
  protected void IsInteractableChanged(bool previousValue, bool newValue)
  {
    isInteractable = newValue;
  }

  //Called on frame when first loaded
  protected virtual void Start()
  {
    hoverText.SetActive(false);
    if (baseParent == null)
    {
      baseParent = this.transform.parent.gameObject;
    }
    else
    {
      if(NetworkManager.Singleton == null){
        //Used to do things. No longer needed, but left due to laziness.
      }
      else if(NetworkManager.Singleton.IsServer){
        transform.SetParent(baseParent.transform);
      }
    }
    equippedTo = baseParent;
    GetPosRotServerRpc();
  }

  /// <summary>
  /// Meant to fix floating object bug.
  /// Gets the server side location of the object and forces the client to match.
  /// </summary>
  [ServerRpc(RequireOwnership =false)]
  private void GetPosRotServerRpc()
  {
    SetPosRotClientRpc(transform.position, transform.eulerAngles);
  }

  /// <summary>
  /// Forces the client side of an object to match the server.
  /// used to fix the floating object bug.
  /// </summary>
  /// <param name="position"></param>
  /// <param name="eulerAngles"></param>
  [ClientRpc]
  private void SetPosRotClientRpc(Vector3 position, Vector3 eulerAngles)
  {
    transform.position = position;
    transform.eulerAngles = eulerAngles;
  }

  //Called Every Frame
  //Usually handles clicking and dragging
  //Should always call ConnectionStart
  //Update rigidbody server side.
  protected virtual void Update()
  {
    ConnectionStart();
    //Dragging object.
    if (drag)
    {
      Vector3 camPos = pseudoCamCurrent.transform.position;

      //NOTE: Dragdist not working properly. Bug.
      Vector3 newPos = pseudoCamCurrent.transform.position + pseudoCamCurrent.transform.forward * dragDist;
      //Debug.Log(newPos);
      repositionGlobalServerRpc(newPos, transform.eulerAngles);
      if (NetworkManager.Singleton == null)
      {
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Rigidbody>().detectCollisions = false;
        GetComponent<Rigidbody>().useGravity = false;
      }
      else if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsClient)
      {
        updateRigidBodyServerRpc(false);
      }
    }

    if(NetworkManager.Singleton == null && Connected){
      Destroy(gameObject);
    }

  }

  /// <summary>
  /// Called when object first spawns on server
  /// </summary>
  protected virtual void ConnectionStart()
  {
    if(NetworkManager.Singleton != null){
      if(!Connected && NetworkManager.Singleton.IsServer){
        transform.SetParent(baseParent.transform);
        netIsInteractable.Value = isInteractable;
        netEquippedTo.Value = baseParent.GetComponent<NetworkObject>();
        Connected = true;
      }
    }
  }

  //Used to set the object this is equipped to
  public void SetEquippedTo(GameObject o)
  {
    equippedTo = o;
    if(NetworkManager.Singleton != null){
      if(NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsClient){
        updateEquippedToServerRpc(o.GetComponent<NetworkObject>());
      }
    }
    
  }

}
