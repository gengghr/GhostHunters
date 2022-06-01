using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles objects the ghost can interact with.
/// Author: Brandon Hullinger
/// Build: Beta
/// </summary>
public abstract class GhostInteractable : MonoBehaviour
{

  public bool isInteractable;
  public Vector3 highPoint = Vector3.zero;
  public float sensedMemTime;
  public float seenMemTime;
  public float interactMaxDelay;
  protected float interactDelay;


  //Called when object in ghost sense range
  //Default Case is just adding this object to ghost's Known Targets.
  //Returns True if action succeeded.
  public virtual bool Sensed(GameObject caller){
    GhostInteraction GO = caller.GetComponent<GhostInteraction>();
    GO.knownTargets[gameObject] = new System.Tuple<Transform, float>(this.transform, seenMemTime);
    return true;
  }

  //Called when ghost sees object
  //Default Case is just adding this object to ghost's Known Targets.
  //Returns True if Action Succeeded.
  public virtual bool Seen(GameObject caller){
    GhostInteraction GO = caller.GetComponent<GhostInteraction>();
    GO.knownTargets[gameObject] = new System.Tuple<Transform, float>(this.transform, seenMemTime);
    return true;
  }

  //Called when ghost interacts with object
  //Default Case is to do nothing. This should be done on a case by case basis.
  //Returns True if action succeeds.
  public virtual bool Triggered(GameObject caller){
    return false;
  }

  protected virtual void Update(){
    if(interactDelay > 0){
      interactDelay -= Time.deltaTime;
    }
    else{
      isInteractable = true;
    }
  }

  protected virtual void Start()
  {
    //do nothing
  }


}
