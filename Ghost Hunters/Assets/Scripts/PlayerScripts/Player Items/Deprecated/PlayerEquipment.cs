using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Handles player inventory and calling equipped item functions.
//Deprecated.
public class PlayerEquipment : MonoBehaviour
{
  public GameObject[] Equipped;
  public int inHand;
    //public float scale = 1f;
  // Start is called before the first frame update
  void Start()
  {
    Equipped = new GameObject[1];
    inHand = 0;
  }

  // Update is called once per frame
  void Update()
  {
    //Check there's something held in hand
    if(Equipped[inHand] != null){
      if(Input.GetKeyDown(KeyCode.Q)){
        //Call the equipped item's drop function.
        Equipped[inHand].GetComponent<PlayerInteractable>().Drop(this.gameObject);
      }
      if (Input.GetMouseButtonDown(1))
      {
        //Call the equipped item's use function.
        Equipped[inHand].GetComponent<PlayerInteractable>().Use(this.gameObject);
      }
      if (Input.GetKeyDown(KeyCode.F))
      {
        //Call equipped item's place function.
        Equipped[inHand].GetComponent<PlayerInteractable>().Place(this.gameObject);
      }
    }
    //Todo: Allow switching which item is held. For now, one item max.
  }

  //Default Equip Function. Equips the given object (toEquip) to the designated slot, then positions it relative to the holder at a given position (holdLoc) and angle (holdRot)
  //Expects no physics on equipped object, so make sure to disable it before calling this function.
  internal void Equip(GameObject toEquip, Vector3 holdLoc, Vector3 holdRot, int slot, Vector3 scale)
  {
    //Check that the slot is actually empty.
    //Debug.Log("Entered Equip");
    if(CanEquip(slot)){
            //Essentially 0 outthe object's parameters.
            //toEquip.transform.localScale = new Vector3(toEquip.transform.localScale.x / transform.localScale.x, toEquip.transform.localScale.y / transform.localScale.y,
            // toEquip.transform.localScale.z / transform.localScale.z);
      toEquip.transform.localScale = scale;
      toEquip.transform.eulerAngles = new Vector3(0, 0, 0);
      toEquip.transform.position = new Vector3(0, 0, 0);
      //Set player as parent object.
      toEquip.transform.SetParent(transform, false);
      toEquip.transform.localPosition = holdLoc;
      toEquip.transform.localEulerAngles = holdRot;
      Equipped[slot] = toEquip;
    }
  }

  //Gets if there is an empty slot, and if so, return the empty slot.
  public bool CanEquip(out int slot){
    //Debug.Log("Testing Equipability");
    for(int i = 0; i < Equipped.Length;i++){
      if(Equipped[i] == null){
        slot = i;
        return true;
      }
    }
    slot = -1;
    return false;
  }

  //Gets if a given slot is empty.
  public bool CanEquip(int slot){
    if(Equipped[slot] == null){
      return true;
    }
    return false;
  }
}
