using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Handles Ghost Interacting With Objects in Multiplayer
/// Does not work
/// Left because causes bugs if we don't.
/// Author: Brandon Hullinger
/// Build: Final
/// </summary>
public class NetGhostInteraction : NetworkBehaviour
{

  public Vector3 ghostEyePos = Vector3.zero;
  public float senseDist;

  public float viewAngle;

  public float triggerDist;
  public Vector3 triggerBoxBounds = Vector3.zero;

  //GameObject, Last Sensed Location, Time Until Target Falls out of Memory.
  public Dictionary<GameObject, Tuple<Transform, float>> knownTargets;

  // Start is called before the first frame update
  protected virtual void Start()
  {
    knownTargets = new Dictionary<GameObject, Tuple<Transform, float>>();
  }

  // Update is called once per frame
  protected virtual void Update()
  {
    if (NetworkManager.Singleton == null || NetworkManager.Singleton.IsServer)
    {
      foreach (GameObject go in knownTargets.Keys)
      {
        Transform lastLocation = knownTargets[go].Item1;
        float timeLeft = knownTargets[go].Item2 - Time.deltaTime;
        if (timeLeft <= 0)
        {
          knownTargets.Remove(go);
        }
        else
        {
          knownTargets[go] = new Tuple<Transform, float>(lastLocation, timeLeft);
        }
      }

      RaycastHit[] sensed = Physics.SphereCastAll(this.transform.position + ghostEyePos, senseDist, this.transform.forward, 0f);
      foreach (RaycastHit hit in sensed)
      {
        if (hit.transform.gameObject.GetComponent<NetGhostInteractable>() != null)
        {
          hit.transform.gameObject.GetComponent<NetGhostInteractable>().Sensed(this.gameObject);
          if (Vector3.Angle(this.transform.position, hit.transform.position) < viewAngle)
          {
            Vector3 objectHeight = hit.transform.GetComponent<NetGhostInteractable>().highPoint;
            Vector3 dir = transform.position + ghostEyePos - hit.transform.position - objectHeight;
            dir.Normalize();
            Physics.Raycast(this.transform.position + ghostEyePos, dir, out RaycastHit seen);
            if (seen.transform.gameObject == hit.transform.gameObject)
            {
              hit.transform.gameObject.GetComponent<NetGhostInteractable>().Seen(this.gameObject);
            }
          }
        }
      }


      RaycastHit[] triggered = Physics.BoxCastAll(transform.position + ghostEyePos, triggerBoxBounds, transform.forward, Quaternion.LookRotation(transform.forward, transform.up), triggerDist);

      BoxCastDebug.DrawBoxCastBox(transform.position + ghostEyePos, triggerBoxBounds, Quaternion.LookRotation(transform.forward, transform.up), transform.forward, triggerDist, Color.blue);

      foreach (RaycastHit h in triggered)
      {
        if (h.transform.gameObject.GetComponent<NetGhostInteractable>() != null)
        {
          //Debug.Log("Hit " + h.transform.gameObject);
          h.transform.gameObject.GetComponent<NetGhostInteractable>().Triggered(this.gameObject);
        }
      }
    }

  }
}
