using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles Setting up doors in single player
/// Author: Brandon Hullinger
/// Build: Beta
/// </summary>
public class SetupDoors : MonoBehaviour
{
  public GameObject doorHoverText;
  public GameObject doorBaseParent;
  public RuntimeAnimatorController cont;

  // Start is called before the first frame update
  void Start()
  {
    //Gets List of Doors and Adds requires scripts and components.
    GameObject[] doors = GameObject.FindGameObjectsWithTag("Doors_Agg");
    foreach(GameObject d in doors){
      if(d.TryGetComponent(out DoorOpenScript dos)){
        //Debug.Log("Disabling DoorOpenScript");
        dos.enabled = false;
      }
      else{
        //Debug.Log("No DoorOpenScript");
      }
      if(!d.TryGetComponent(out Animator dan)){
        dan = d.AddComponent<Animator>();
        dan.runtimeAnimatorController = cont;
      }
      DoorInteractible dis = d.AddComponent<DoorInteractible>();
      dis.hoverText = GameObject.Instantiate(doorHoverText);
      dis.baseParent = d.transform.parent.gameObject;
      d.transform.parent.SetParent(doorBaseParent.transform);
      dis.hoverTextPos = new Vector3(0,1.3f,0);
      dis.hoverTextClose = 0.2f;
      dis.animator = d.GetComponent<Animator>();

      GhostInteractibleDoor gid = d.AddComponent<GhostInteractibleDoor>();
      gid.interactMaxDelay = 1.1f;
      gid.animator = d.GetComponent<Animator>();
    }
  }

  // Update is called once per frame
  void Update()
  {

  }
}
