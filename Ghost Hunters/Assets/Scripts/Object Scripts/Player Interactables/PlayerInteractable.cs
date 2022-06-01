using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//Provides the default implementation of an interactible item class.
//Handles the items side of player-item interaction.
/// <summary>
/// Author: Brandon Hullinger
/// Build: Beta
/// </summary>
public class PlayerInteractable : MonoBehaviour
{
  public bool isInteractable = true;
  public Vector3 equipLoc = new Vector3(0f, 0f, 0f);
  public Vector3 equipRot = new Vector3(0f, 0f, 0f);
  protected bool drag = false;
  public float dragDist = 1f;
  protected Camera currentCam;
  public GameObject hoverText;
  public Vector3 hoverTextPos;
  public float hoverTextClose;
  protected GameObject equippedTo = null;
  public GameObject baseParent;

  public static List<string> intangibleTags = new List<string>() { "Item", "BoundItem", "ProtectionCircle", "ProtectionCircleCandleSlot" };

  //Called when player hovers reticule over object.
  //Returns True if action succeeded. False otherwise.
  public virtual bool Hover(GameObject caller)
  {
    Camera cam = caller.GetComponent<PlayerInteraction>().playerCam;
    Vector3 itemWorldPos = transform.position;
    hoverText.transform.LookAt(cam.transform);
    hoverText.transform.rotation = Quaternion.LookRotation(cam.transform.forward) * Quaternion.Euler(90, 0, 0);
    hoverText.transform.position = itemWorldPos + hoverTextPos + hoverText.transform.up * hoverTextClose * -1;
    hoverText.SetActive(true);
    return true;
  }

  //Called when player stops hovering reticule over object.
  //Returns True if action succeeded. False otherwise.
  public virtual bool StopHover(GameObject caller)
  {
    hoverText.SetActive(false);
    return true;
  }

  //Called when player clicks on object
  //Base Case, item is picked up and carried. Not Equipped.
  //Returns True if action succeeded. False otherwise.
  public virtual bool Click(GameObject caller)
  {
    if (!isInteractable)
      return false;
    //Activate Dragging Object
    //Set object as draggable and set position relative to screen to hold object at.
    gameObject.GetComponent<Rigidbody>().useGravity = false;
    //gameObject.GetComponent<Rigidbody>().detectCollisions = false;
    currentCam = caller.GetComponent<PlayerInteraction>().playerCam;
    drag = true;
    isInteractable = false;
    return true;
  }

  //Called when player releases clicking on an object
  //Base Case, stop carrying clicked object
  //Returns True if action succeeded. False otherwise.
  public virtual bool ReleaseClick(GameObject caller)
  {
    gameObject.GetComponent<Rigidbody>().useGravity = true;
    this.GetComponent<Rigidbody>().velocity = Vector3.zero;
    this.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    gameObject.GetComponent<Rigidbody>().detectCollisions = true;
    drag = false;
    isInteractable = true;
    return true;
  }

  //Called when player presses 'Interact" key (Default E)
  //Base Case, item is equipped if there is an empty equipment slot.
  //Returns True if action succeeded. False otherwise.
  public virtual bool Interact(GameObject caller)
  {
    if (isInteractable && caller.GetComponent<PlayerInteraction>().CanEquip(out int equipSlot))
    {
      gameObject.GetComponent<Rigidbody>().detectCollisions = false;
      gameObject.GetComponent<Rigidbody>().useGravity = false;
      gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
      gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
      isInteractable = false;
      transform.eulerAngles = new Vector3(0, 0, 0);
      transform.position = new Vector3(0, 0, 0);
      transform.SetParent(caller.transform);
      transform.localPosition = equipLoc;
      transform.localEulerAngles = equipRot;
      caller.GetComponent<PlayerInteraction>().Equipped[equipSlot] = this.gameObject;
      equippedTo = caller;
      return true;
    }
    return false;
  }

  //Called when player uses item while equpped
  //Returns True if action succeeded. False otherwise.
  public virtual bool Use(GameObject caller)
  {
    //Implement on an item by item basis.
    return true;
  }

  //Called when player places item in the world
  //Returns True if action succeeded. False otherwise.
  public virtual bool Place(GameObject caller)
  {
    //Called on item by item basis.
    //Most should not have a 'place' attribute.
    return true;
  }

  //Called when player drops the item without placing
  //Base: Let go of equipped item and let it drop.
  //Returns True if action succeeded. False otherwise.
  public virtual bool Drop(GameObject caller)
  {
    //Debug.Log("Dropping");
    this.GetComponent<PlayerInteractable>().isInteractable = true;
    caller.GetComponent<PlayerInteraction>().Equipped[caller.GetComponent<PlayerInteraction>().inHand] = null;
    this.transform.SetParent(baseParent.transform);
    this.GetComponent<Rigidbody>().detectCollisions = true;
    this.GetComponent<Rigidbody>().useGravity = true;
    equippedTo = null;
    return true;
  }

  //Called on frame when first loaded
  protected virtual void Start()
  {
    hoverText.SetActive(false);
    if (baseParent == null)
    {
      baseParent = transform.parent.gameObject;
    }
  }

  //Called Every Frame
  //Usually handles clicking and dragging
  protected virtual void Update()
  {

    //Dragging object.
    if (drag && Mouse.current.leftButton.IsPressed())
    {
      Vector3 camPos = currentCam.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0));

      float hDist = dragDist;
      //RaycastHit[] hit = Physics.RaycastAll(camPos, currentCam.transform.forward, dragDist);
      //HashSet<string> intangibleSet = new HashSet<string>(intangibleTags);
      //foreach (RaycastHit h in hit)
      //{
      //  if (!intangibleSet.Contains(h.transform.gameObject.tag))
      //  {
      //    if (h.distance < hDist)
      //    {
      //      hDist = h.distance;
      //    }
      //  }
      //}

      Vector3 newPos = currentCam.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, hDist));
      transform.position = newPos;
      this.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
      //Debug.Log("Dragging");
    }
    if (drag && Mouse.current.leftButton.wasReleasedThisFrame)
    {
      ReleaseClick(null);
    }

  }

  //Used to set the object this is equipped to
  public void SetEquippedTo(GameObject o)
  {
    equippedTo = o;
  }

}
