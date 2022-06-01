using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//WIP. Meant to handle creating salt lines. Extends EquipableItem
public class SaltCanEO : EquipableItem
{
  public int salt = 10;

  new public void UseObject()
  {
    if (salt <= 0) return;
    Debug.Log("Using Salt Can");
    salt--;
  }
}
