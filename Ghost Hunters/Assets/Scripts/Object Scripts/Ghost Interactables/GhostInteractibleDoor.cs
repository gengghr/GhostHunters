using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the doors interacting with ghosts.
/// Author: Brandon Hullinger
/// Build: Beta
/// </summary>
public class GhostInteractibleDoor : GhostInteractable
{
  public Animator animator; //Animator for door
  public override bool Seen(GameObject caller)
  {
    return false;
  }

  public override bool Sensed(GameObject caller)
  {
    return false;
  }

  protected override void Start()
  {
    base.Start();
    animator = GetComponent<Animator>();
  }

  protected override void Update()
  {
    base.Update();
  }

  /// <summary>
  /// Opens the doors
  /// </summary>
  /// <param name="caller"></param>
  /// <returns></returns>
  public override bool Triggered(GameObject caller)
  {
    //Debug.Log("Door " + gameObject.name + "Triggered");
    if(isInteractable){
      animator.SetBool("open", !animator.GetBool("open"));
      //animator.SetBool("open", true);
      interactDelay = interactMaxDelay;
      isInteractable = false;
      return true;
    }
    return false;
  }
}
