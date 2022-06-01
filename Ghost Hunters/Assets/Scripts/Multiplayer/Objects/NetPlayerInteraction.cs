using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles the player side of player-object interactions in Multiplayer
/// Author: Brandon Hullinger
/// Build: Final
/// </summary>
public class NetPlayerInteraction : NetworkBehaviour
{
  public float dist = 5.0f;
  public GameObject pseudoCam;
  public GameObject Settings = null;
  private GameObject hovering = null;
  private GameObject clicking = null;
  public int invSize = 2;
  public List<string> ignoreTags = new List<string>();
  public List<GameObject> ignoreObjects = new List<GameObject>();

  //Variables that need to be syncronized
  public List<GameObject> Equipped = new List<GameObject>();
  public int inHand = 0;

  //Network Variables
  private NetworkList<NetworkObjectReference> netEquipped;
  private NetworkVariable<int> netInHand = new NetworkVariable<int>();

  private void Awake()
  {
    ignoreObjects.Add(gameObject);
    netEquipped = new NetworkList<NetworkObjectReference>();
    netInHand.OnValueChanged += inHandChanged;
    netEquipped.OnListChanged += equippedChanged;
  }

  // Start is called before the first frame update
  void Start()
  {

  }

  /// <summary>
  /// Called when netEquipped changes.
  /// Updates the player's equipment across all clients.
  /// </summary>
  /// <param name="changeEvent"></param>
  private void equippedChanged(NetworkListEvent<NetworkObjectReference> changeEvent)
  {
    foreach (GameObject o in Equipped)
    {
      o.SetActive(true);
    }
    Equipped = new List<GameObject>();
    foreach (NetworkObjectReference nwor in netEquipped)
    {
      if (nwor.TryGet(out NetworkObject netObj))
      {
        Equipped.Add(netObj.gameObject);
      }
    }
    for (int i = 0; i < Equipped.Count; i++)
    {
      if (i != inHand)
      {
        Equipped[i].SetActive(false);
      }
    }
    if (inHand >= Equipped.Count)
    {
      if (Equipped.Count == 0)
      {
        inHand = -1;
      }
      else
      {
        inHand = 0;
      }
    }
    if(IsOwner){
      UpdateInHandServerRpc(inHand);
    }
  }

  /// <summary>
  /// Called when netInHand changes
  /// Updates which object the player is holding across all clients.
  /// </summary>
  /// <param name="previousValue"></param>
  /// <param name="newValue"></param>
  private void inHandChanged(int previousValue, int newValue)
  {
    if (inHand != newValue)
    {
      inHand = newValue;
      ChangeInHandObject();
    }
  }

  // Update is called once per frame
  void Update()
  {
    if (NetworkManager.Singleton == null)
    {
      DoClientUpdate();
    }
    else if (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsServer)
    {

      if (IsOwner)
      {
        DoClientUpdate();
        NetworkObjectReference[] equipRefs = new NetworkObjectReference[Equipped.Count];
        for (int i = 0; i < Equipped.Count; i++)
        {
          Equipped[i].SetActive(true);
          equipRefs[i] = Equipped[i].GetComponent<NetworkObject>();
          Equipped[i].SetActive(i == inHand);
        }
        UpdateInHandServerRpc(inHand);
        UpdateEquippedServerRpc(equipRefs);
      }
    }
  }

  /// <summary>
  /// Updates netInHand server side when client inHand changes.
  /// </summary>
  /// <param name="newInHand"></param>
  [ServerRpc]
  public void UpdateInHandServerRpc(int newInHand)
  {
    if (netInHand.Value != newInHand)
    {
      netInHand.Value = newInHand;
    }
  }

  /// <summary>
  /// Updates netEquipped Server Side when client equipment changes.
  /// </summary>
  /// <param name="equipRefs"></param>
  [ServerRpc]
  public void UpdateEquippedServerRpc(NetworkObjectReference[] equipRefs)
  {
    if (netEquipped.Count == equipRefs.Length)
    {
      for (int i = 0; i < netEquipped.Count; i++)
      {
        if (!netEquipped[i].Equals(equipRefs[i]))
        {
          netEquipped.Clear();
          foreach (NetworkObjectReference nwor in equipRefs)
          {
            netEquipped.Add(nwor);
          }
          return;
        }
      }
    }
    else
    {
      netEquipped.Clear();
      foreach (NetworkObjectReference nwor in equipRefs)
      {
        netEquipped.Add(nwor);
      }
    }
  }

  /// <summary>
  /// Does client side calculations
  /// </summary>
  private void DoClientUpdate()
  {
    EndHover();
    GetNewHover();
    DoHover();
    DoInteract();
    DoClick();
    DoReleaseClick();
    DoUse();
    DoPlace();
    DoDrop();
    DoHandSwitch();
  }

  /// <summary>
  /// Stops the hover action.
  /// Ends tooltip display, usually.
  /// </summary>
  private void EndHover()
  {
    if (hovering != null)
    {
      hovering.GetComponent<NetPlayerInteractable>().StopHover(gameObject);
      hovering = null;
    }
  }

  /// <summary>
  /// Gets new object that the player is looking at.
  /// </summary>
  private void GetNewHover()
  {
    Vector3 camPos = pseudoCam.transform.position;
    Vector3 camDir = pseudoCam.transform.forward;
    if (RayCastIgnore(camPos, camDir, out RaycastHit hit, dist, new HashSet<string>(ignoreTags), new HashSet<GameObject>(ignoreObjects)))
    {
      if (hit.transform.gameObject.GetComponent<NetPlayerInteractable>() != null)
      {
        hovering = hit.transform.gameObject;
      }
    }
  }

  /// <summary>
  /// Does the hover action.
  /// Usually displaying tooltip.
  /// </summary>
  private void DoHover()
  {
    if (hovering != null)
    {
      hovering.GetComponent<NetPlayerInteractable>().Hover(gameObject);
    }
  }

  /// <summary>
  /// Called when player presses E on object.
  /// Usually equips object.
  /// </summary>
  private void DoInteract()
  {
    if (Keyboard.current[Key.E].wasPressedThisFrame && hovering != null)
    {
      hovering.GetComponent<NetPlayerInteractable>().Interact(gameObject);
    }
  }

  /// <summary>
  /// Called when player clicks while hovering over object
  /// Usually does click and drag.
  /// </summary>
  private void DoClick()
  {
    if (Mouse.current.leftButton.wasPressedThisFrame && hovering != null)
    {
      if (hovering.GetComponent<NetPlayerInteractable>().Click(gameObject))
      {
        clicking = hovering;
      }
    }
  }

  /// <summary>
  /// Called when player releases left mouse while hovering over object.
  /// Usually drops dragged object
  /// </summary>
  private void DoReleaseClick()
  {
    if (Mouse.current.leftButton.wasReleasedThisFrame && clicking != null)
    {
      if (clicking.GetComponent<NetPlayerInteractable>().ReleaseClick(gameObject))
      {
        clicking = null;
      }
    }
  }

  /// <summary>
  /// Called when player presses right mouse button with equipped item
  /// </summary>
  private void DoUse()
  {
    if (Mouse.current.rightButton.wasPressedThisFrame && inHand != -1)
    {
      Equipped[inHand].GetComponent<NetPlayerInteractable>().Use(gameObject);
    }
  }

  /// <summary>
  /// Called when player presses F with equipped item.
  /// Does nothing in all cases at present. Was hoping to have some cases that used it.
  /// </summary>
  private void DoPlace()
  {
    if (Keyboard.current[Key.F].wasPressedThisFrame && inHand != -1)
    {
      Equipped[inHand].GetComponent<NetPlayerInteractable>().Place(gameObject);
    }
  }

  /// <summary>
  /// Called when player presses Q while having equipped object in hand.
  /// Drops object.
  /// </summary>
  public void DoDrop()
  {
    if (Keyboard.current[Key.Q].wasPressedThisFrame && inHand != -1)
    {
      Equipped[inHand].GetComponent<NetPlayerInteractable>().Drop(gameObject);
    }
  }

  /// <summary>
  /// Handles scrolling up and down to change held item.
  /// </summary>
  public void DoHandSwitch()
  {
    if (Equipped.Count > 0)
    {
      if (inHand < 0)
      {
        inHand = 0;
      }
      if (Mouse.current.scroll.y.ReadValue() != 0)
      {
        inHand += Math.Sign(Mouse.current.scroll.y.ReadValue());
        if (inHand >= Equipped.Count)
        {
          inHand = 0;
        }
        if (inHand < 0)
        {
          inHand = Equipped.Count - 1;
        }
      }
    }
    else{
      inHand = -1;
    }

    ChangeInHandObject();
  }

  /// <summary>
  /// Handles changing the actual held item
  /// </summary>
  private void ChangeInHandObject()
  {
    for (int i = 0; i < Equipped.Count; i++)
    {
      Equipped[i].SetActive(i == inHand);
    }
  }




  //Gets if there is an empty slot, and if so, output the empty slot.
  public bool CanEquip()
  {
    for (int i = 0; i < invSize; i++)
    {
      if (i >= Equipped.Count)
      {
        return true;
      }
    }
    return false;
  }

  //Gets closest hit object using raycast ignoring given tags.
  public static bool RayCastIgnore(Vector3 origin, Vector3 direction, out RaycastHit hit, float distance, HashSet<string> tagsToIgnore = null, HashSet<GameObject> objsToIgnore = null)
  {
    bool hitSomething = false;
    hit = new RaycastHit();
    RaycastHit[] hitList = Physics.RaycastAll(origin, direction, distance);
    float hitDist = float.PositiveInfinity;
    foreach (RaycastHit h in hitList)
    {
      if (!(tagsToIgnore is null) && !tagsToIgnore.Contains(h.transform.gameObject.tag) && !(objsToIgnore is null) && !objsToIgnore.Contains(h.transform.gameObject))
      {
        if (h.distance < hitDist)
        {
          hitDist = h.distance;
          hitSomething = true;
          hit = h;
        }
      }
    }
    return hitSomething;
  }

}
