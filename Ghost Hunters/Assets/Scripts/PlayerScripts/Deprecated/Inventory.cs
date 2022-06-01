using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Handles prototype inventory.
//Deprecated. Left to prevent issues. Use PlayerEquipment class instead.
public class Inventory : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject equipped = null;
    public Camera cam;
    private GameObject lastHit;
    public float dist = 0f;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 camCenter = cam.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, cam.nearClipPlane));
        RaycastHit hit;
        if (Physics.Raycast(camCenter, cam.transform.forward, out hit, dist)) //Raycast out from center of screen.
        {
            if(hit.transform.gameObject.GetComponent<Interactable>() != null)
            {
                lastHit = hit.transform.gameObject;
            }
            else
            {
                lastHit = null;
            }
        }
        else
        {
            lastHit = null;
        }

        //Drops equipped object in front of player.
        if (equipped != null)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                //Item Drop Position
                equipped.transform.position = this.transform.forward + cam.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 2));
                //Item drop direction facing.
                equipped.transform.eulerAngles = this.transform.eulerAngles + new Vector3(0, 270, -90);
                equipped.transform.parent = null;
                equipped.GetComponent<Rigidbody>().detectCollisions = true;
                equipped.GetComponent<Rigidbody>().useGravity = true;
                equipped = null;
            }

            if (Input.GetMouseButtonDown(1))
            {

            }
        }

        if(lastHit == null)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {

        }

        else if (Input.GetMouseButton(0))
        {

        }

        else if (Input.GetMouseButtonUp(0))
        {

        }

        if (Input.GetKeyDown(KeyCode.E))
        {

        }

        
    }
}
