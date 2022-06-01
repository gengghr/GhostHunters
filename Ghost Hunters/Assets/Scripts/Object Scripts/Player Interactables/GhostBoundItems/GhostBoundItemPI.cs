using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles Ghost Bound Object interaction with Player and Protection Circle
/// Author: Brandon Hullinger
/// Build: Beta
/// </summary>
public class GhostBoundItemPI : PlayerInteractable
{
    [SerializeField]
  public GameObject protectionCircle;
  public float protectionCircleRadius;
  public float protectionCircleHeight;

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
    if (baseParent == null)
    {
            baseParent = this.transform.parent.gameObject;
        }
    else
    {
            transform.SetParent(baseParent.transform);
        }
  }

  /// <summary>
  /// Checks if in protection circle and then ignites.
  /// </summary>
  protected override void Update()
  {
    base.Update();

    if (drag || isInteractable)
    {
      Vector3 XZthis = new Vector3(this.transform.position.x, 0, this.transform.position.z);
      Vector3 XZprotCircle = new Vector3(protectionCircle.transform.position.x, 0, protectionCircle.transform.position.z);
      float dist = Vector3.Distance(XZthis, XZprotCircle);
      float height = this.transform.position.y - protectionCircle.transform.position.y;
      if (TryGetComponent<GhostBoundIgnite>(out GhostBoundIgnite gbi) && dist <= protectionCircleRadius && height > 0 && height < protectionCircleHeight)
      {
        ReleaseClick(gameObject);
        isInteractable = false;
        gameObject.GetComponent<Flammable>().Ignite();
      }
    }
  }
}
