using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

/// <summary>
/// Handles how Bound Items work with Player and Protection Circle in Multiplayer
/// Author: Brandon Hullinger
/// Build: Final
/// </summary>
public class NetGhostBoundItemPI : NetPlayerInteractable
{
    [SerializeField]
  public GameObject protectionCircle;
  public float protectionCircleRadius;
  public float protectionCircleHeight;
  private bool Connected2 = false;

  public override bool Interact(GameObject caller)
  {
    return false;
  }

  public override bool Drop(GameObject caller)
  {
    return false;
  }

  protected override void Start()
  {
    Connected = true;
  }

  protected override void Update()
  {

    ConnectionStart();
    base.Update();

    //Checks if Bound Object is within Protection Circle and ignites if so.
    if (drag || isInteractable)
    {
      Vector3 XZthis = new Vector3(this.transform.position.x, 0, this.transform.position.z);
      Vector3 XZprotCircle = new Vector3(protectionCircle.transform.position.x, 0, protectionCircle.transform.position.z);
      float dist = Vector3.Distance(XZthis, XZprotCircle);
      float height = this.transform.position.y - protectionCircle.transform.position.y;
      if (TryGetComponent<NetGhostBoundIgnite>(out NetGhostBoundIgnite gbi) && dist <= protectionCircleRadius && height > 0 && height < protectionCircleHeight)
      {
        ReleaseClick(gameObject);
        isInteractable = false;
        gameObject.GetComponent<NetFlammable>().Ignite();
      }
    }
  }

  protected override void ConnectionStart()
  {
    if (NetworkManager.Singleton != null)
    {
      if (!Connected2 && NetworkManager.Singleton.IsServer && baseParent != null)
      {
        transform.SetParent(baseParent.transform);
        netIsInteractable.Value = isInteractable;
        netEquippedTo.Value = baseParent.GetComponent<NetworkObject>();
        Connected2 = true;
      }
    }
  }
}
