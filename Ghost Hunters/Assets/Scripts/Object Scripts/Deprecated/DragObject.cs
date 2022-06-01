using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Old handling of drag objects
/// Author: Brandon Hullinger
/// Build: Prototype
/// </summary>
public class DragObject : MonoBehaviour
{

    private Vector3 myOffset;
    private float myZcoord;
    public Camera cam;
    private bool drag;
    public float dist = 5f;

    void Start()
    {
        drag = false;
    }
    void Update()
    {
        //Set object as draggable and set position relative to screen to hold object at.
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            Vector3 camCenter = cam.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, cam.nearClipPlane));
            RaycastHit hit;
            if (Physics.Raycast(camCenter, cam.transform.forward, out hit, dist)) //Raycast out from center of screen.
            {
                //Debug.Log("Hit " + hit.transform.gameObject);
                if (hit.transform.gameObject == gameObject)
                {
                    drag = true;
                    gameObject.GetComponent<Rigidbody>().useGravity = false;
                    gameObject.GetComponent<Rigidbody>().detectCollisions = false;
                    myZcoord = cam.WorldToScreenPoint(gameObject.transform.position).z;
                    myOffset = gameObject.transform.position - GetCenterScreenWorldPos();
                    Debug.Log("Dragging");
                }
            }
        }
        //Dragging object.
        if (drag && Mouse.current.leftButton.isPressed)
        {
            transform.position = GetCenterScreenWorldPos() + myOffset;
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            gameObject.GetComponent<Rigidbody>().useGravity = true;
            gameObject.GetComponent<Rigidbody>().detectCollisions = true;
            drag = false;
        }



    }

    // when you release the button
    void OnMouseDown()
    {
        /*
        //Debug.Log("onmousedown");
        myZcoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        myOffset = gameObject.transform.position - GetMouseWorldPos();
        */
    }

    //Gets the position of the center of the screen in the world, then adds the distance away to hold the object.
    private Vector3 GetCenterScreenWorldPos()
    {
        Vector3 screenPos = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        screenPos.z = myZcoord;
        return Camera.main.ScreenToWorldPoint(screenPos);
    }

    void OnMouseDrag()
    {

        /*
        transform.position = GetMouseWorldPos() + myOffset;
        */
    }
}
