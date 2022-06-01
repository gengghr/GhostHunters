using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Deprecated. Do Not use.
public class FlashlightEquipable : PlayerEquipable
{
  private Vector3 scale;
  private bool flashLightOn = true;
  public void Start()
  {
    scale = gameObject.transform.localScale;
  }

  //Drop flashlight a short distance in front of player
  //Maybe make this into a default function.
  public override void Drop(GameObject caller)
  {
    //Debug.Log("Testing Dropping");
    //Item Drop Position
    //this.transform.position = caller.transform.forward + caller.GetComponent<PlayerInteraction>().playerCam.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 1.5f*caller.transform.localScale.z));
    //Item drop direction facing.
    //this.transform.eulerAngles = this.transform.eulerAngles + new Vector3(0, 270, -90);
    this.transform.parent = null;
    //this.transform.localScale = scale;
    this.GetComponent<Rigidbody>().detectCollisions = true;
    this.GetComponent<Rigidbody>().useGravity = true;
    caller.GetComponent<PlayerEquipment>().Equipped[caller.GetComponent<PlayerEquipment>().inHand] = null;
    this.GetComponent<PlayerInteractable>().isInteractable = true;
  }

  public override void Place(GameObject caller)
  {
    //Unplaceable for now
  }

  //Turn flashlight on or off.
  public override void Use(GameObject caller)
  {
    if(flashLightOn){
      transform.GetChild(0).gameObject.SetActive(false);
    }
    else{
      transform.GetChild(0).gameObject.SetActive(true);
    }
    flashLightOn = !flashLightOn;
  }
}
