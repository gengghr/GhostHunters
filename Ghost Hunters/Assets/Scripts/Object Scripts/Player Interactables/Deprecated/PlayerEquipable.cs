using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Abstract Class for Equippable Objects.
//These functions must be implemented. Can also implement Start/Update functions as needed.
//DEPRECATED - Do not use.
public abstract class PlayerEquipable : MonoBehaviour
{
  //Called when player uses item while equpped
  public abstract void Use(GameObject caller);

  //Called when player places item in the world
  public abstract void Place(GameObject caller);

  //Called when player drops the item without placing
  public abstract void Drop(GameObject caller);


}
