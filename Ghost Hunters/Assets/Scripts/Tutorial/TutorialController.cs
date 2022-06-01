using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the Tutorial Logic
/// Author: Brandon hullinger
/// Build: Final
/// </summary>
public class TutorialController : MonoBehaviour
{
  public int tutStage;
  public Text textObj;
  public GameObject player;
  private Vector3 playerStart;
  private Vector3 playerStartRot;

  public GameObject flashLight;
  public GameObject emfReader;
  private float emfTime = 10;
  public GameObject ghost;
  public GameObject gbo;
  // Start is called before the first frame update
  void Start()
  {
    tutStage = 0;
    playerStart = player.transform.position;
    playerStartRot = player.transform.eulerAngles;
  }

  // Update is called once per frame
  // Changes the current order for the tutorial.
  void Update()
  {
    GameObject held = player.GetComponent<PlayerInteraction>().Equipped[player.GetComponent<PlayerInteraction>().inHand];
    if(held != null){
      //Debug.Log(held.name);
    }
    switch (tutStage)
    {
      case 0:
        textObj.text = "Use W,A,S,D to Move\nPress LShift to Sprint\nUse Mouse to Look Around";
        if (Vector3.Distance(player.transform.position, playerStart) > 1)
        {
          tutStage = 1;
        }
        break;
        
      case 1:
        textObj.text = "Use W,A,S,D to Move\nPress LShift to Sprint\nUse Mouse to Look Around";
        if(Vector3.Distance(playerStartRot, player.transform.eulerAngles) > 10)
        {
          tutStage = 3;
        }
        break;
      case 3:
        textObj.text = "Go to the table and\npoint your reticule at\nan object.\nNotice the tooltip.";
        if ( held != null && held == flashLight){
          tutStage = 4;
        }
        else if(held != null && held == emfReader){
          tutStage = 5;
        }
        break;
      case 4:
        textObj.text = "You have equipped the\nflashlight\nRight Click to turn\nit On and Off";
        if (flashLight.GetComponent<FlashlightInteractable>().spotLight.activeSelf)
        {
          tutStage = 6;
        }
        break;
      case 5:
        textObj.text = "You have equipped the\nEMF Reader\nPress Q to drop it then\nPick up the Flashlight";
        if(held == null){
          tutStage = 3;
        }
        break;
      case 6:
        textObj.text = "Now, equip the EMF Reader\nAnd hold it by rolling the scroll wheel\nNote the number changes as you move.";
        if (player.GetComponent<PlayerInteraction>().Equipped[player.GetComponent<PlayerInteraction>().inHand] == emfReader)
        {
          tutStage = 7;
        }
        break;
      case 7:
        textObj.text = "Now, equip the EMF Reader\nAnd hold it by rolling the scroll wheel\nNote the number changes as you move.";
        emfTime -= Time.deltaTime;
        if(emfTime <= 0){
          tutStage = 8;
        }
        break;
      case 8:
        textObj.text = "Outside that door we\nhave a relatively harmless\nghost for you.\nTurn on your Flashlight\nAnd see what's out there.";
        if(player.transform.position.z < 0){
          tutStage = 9;
        }
        break;
      case 9:
        textObj.text = "Time to find the ghost.\nUse the EMF reader to get close.\nThe Higher the number, the closer you are.";
        Vector3 targetDir = ghost.transform.position - player.transform.position;
        targetDir.y = 0;
        Vector3 fw = player.transform.forward;
        fw.y = 0;
        float angle = Vector3.Angle(targetDir, fw);
        Debug.Log(angle);
        if (GetComponent<Aggression>().AggressionLevel > 80 && angle < 60 && Vector3.Distance(player.transform.position, ghost.transform.position) < 5f)
        {
          tutStage = 10;
        }
        break;
      case 10:
        textObj.text = "Now that you've seen the ghost, now it's time to kill it.\nUse the EMF reader to figure out which mug the ghost is bound to.\nNear the object, the EMF should be about 3. Note that the ghost's EMF reading can hide this, so lure it far away first.";
        float reading = float.Parse(emfReader.GetComponent<EMFPlayerInteractible>().emfText.text);
        if(Vector3.Distance(player.transform.position,gbo.transform.position) < 1.5 && held != null && held == emfReader && reading < 3.5f){
          tutStage = 11;
        }
        break;
      case 11:
        textObj.text = "You found it. Click to pick it up, then return to the room you started in. Place the item in the center of the pentagram.";
        if (gbo.GetComponent<GhostBoundIgnite>().burning)
        {
          tutStage = 12;
        }
        break;
      default:
        textObj.text = "That should do it. Feel free to go out and watch the ghost die.\n\nTutorial Complete";
        break;
    }
  }
}
