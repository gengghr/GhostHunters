using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Handles adding items to inventory.
/// DEPRECATED. All handled in PlayerInteractable.
/// Author: Brandon Hullinger
/// Build: Prototype
/// </summary>
public class EquipableItem : PlayerInteractable
{
  
  protected override void Start()
  {
    
  }

  public override bool Click(GameObject caller)
  {
    //Probably Does Nothing
    return false;
  }

  public override bool Hover(GameObject caller)
  {
    //TODO: Draw Text Box
    return false;
  }

  //Equip the Flashlight to the player.
  public override bool Interact(GameObject caller)
  {
    return base.Interact(caller);
  }
  public override bool Drop(GameObject caller)
  {
    return base.Drop(caller);
  }

  public override bool Place(GameObject caller)
  {
    return false;
  }

  //Turn flashlight on or off.
  public override bool Use(GameObject caller)
  {
    return false;
  }

}
