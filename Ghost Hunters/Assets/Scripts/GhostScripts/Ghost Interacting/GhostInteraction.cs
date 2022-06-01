using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ghost Interaction - Handles the ghost interacting with objects in the world
/// Only used with Protection Circle
/// Primary Author: Brandon Hullinger
/// Build: Beta
/// </summary>
//Handles Ghost Interacting with Objects
public class GhostInteraction : MonoBehaviour
{

  public Vector3 ghostEyePos = Vector3.zero;  //The eye position for the ghost.
  public float senseDist; //How far the ghost can sense others away.

  public float viewAngle; //Field of View Angle

  public float triggerDist; //How far from the ghost something has to be to be activated by the ghost
  public Vector3 triggerBoxBounds = Vector3.zero; //The box for ghost interaction

  //GameObject, Last Sensed Location, Time Until Target Falls out of Memory.
  public Dictionary<GameObject, Tuple<Transform, float>> knownTargets; //Things the ghost has seen or heard. Unused in final version.

  // Start is called before the first frame update
  protected virtual void Start()
  {
    knownTargets = new Dictionary<GameObject, Tuple<Transform, float>>();
  }

  // Update is called once per frame
  protected virtual void Update()
  {
    //Ghost slowly forgets about things.
    //Unused. Was hopeful.
    foreach(GameObject go in knownTargets.Keys){
      Transform lastLocation = knownTargets[go].Item1;
      float timeLeft = knownTargets[go].Item2 - Time.deltaTime;
      if(timeLeft <= 0){
        knownTargets.Remove(go);
      }
      else{
        knownTargets[go] = new Tuple<Transform, float>(lastLocation, timeLeft);
      }
    }
    
    //Gets objects the ghost can see and hear this frame.
    RaycastHit[] sensed = Physics.SphereCastAll(this.transform.position + ghostEyePos, senseDist, this.transform.forward, 0f);
    foreach (RaycastHit hit in sensed)
    {
      if (hit.transform.gameObject.GetComponent<GhostInteractable>() != null)
      {
        hit.transform.gameObject.GetComponent<GhostInteractable>().Sensed(this.gameObject);
        if (Vector3.Angle(this.transform.position, hit.transform.position) < viewAngle)
        {
          Vector3 objectHeight = hit.transform.GetComponent<GhostInteractable>().highPoint;
          Vector3 dir = transform.position + ghostEyePos - hit.transform.position - objectHeight;
          dir.Normalize();
          Physics.Raycast(this.transform.position + ghostEyePos, dir, out RaycastHit seen);
          if(seen.transform.gameObject == hit.transform.gameObject){
            hit.transform.gameObject.GetComponent<GhostInteractable>().Seen(this.gameObject);
          }
        }
      }
    }


    //Triggers close objects.
    RaycastHit[] triggered = Physics.BoxCastAll(transform.position + ghostEyePos, triggerBoxBounds, transform.forward, Quaternion.LookRotation(transform.forward, transform.up), triggerDist);

    BoxCastDebug.DrawBoxCastBox(transform.position + ghostEyePos, triggerBoxBounds, Quaternion.LookRotation(transform.forward, transform.up), transform.forward, triggerDist, Color.blue);

    foreach(RaycastHit h in triggered){
      if(h.transform.gameObject.GetComponent<GhostInteractable>() != null){
        //Debug.Log("Hit " + h.transform.gameObject);
        h.transform.gameObject.GetComponent<GhostInteractable>().Triggered(this.gameObject);
      }
    }

  }
}
