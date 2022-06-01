using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles the player interacting with objects.
/// Author: Brandon Hullinger
/// Build: Alpha
/// </summary>
public class PlayerInteraction : MonoBehaviour
{
    public float dist = 5.0f;
    public Camera playerCam;
    public GameObject[] Equipped;
    public int invSize = 2;
    public int inHand = 0;
    public GameObject Settings = null;
    private GameObject lastHover = null;
    public List<string> ignoreTags = new List<string>();

    public List<GameObject> ignoreObjects = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        Equipped = new GameObject[invSize];
        ignoreObjects.Add(gameObject);
        playerCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (lastHover != null)
        {
            lastHover.GetComponent<PlayerInteractable>().StopHover(this.gameObject);
            lastHover = null;
        }

        //Handle Interacting with other items.
        Vector3 camCenter = playerCam.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, Camera.main.nearClipPlane));
        if (RayCastIgnore(camCenter, playerCam.transform.forward, out RaycastHit hit, dist, new HashSet<string>(ignoreTags), new HashSet<GameObject>(ignoreObjects))) //Raycast out from center of screen.
        {
            //Debug.Log(hit.transform.name);
            if (hit.transform.gameObject.GetComponent<PlayerInteractable>() != null)
            {
                if (hit.transform.gameObject.GetComponent<PlayerInteractable>().isInteractable)
                {
                    lastHover = hit.transform.gameObject;
                    //Tooltip Text
                    hit.transform.gameObject.GetComponent<PlayerInteractable>().Hover(this.gameObject);
                    //Equipping
                    if (Keyboard.current[Key.E].wasPressedThisFrame)
                    {
                        hit.transform.gameObject.GetComponent<PlayerInteractable>().Interact(this.gameObject);
                    }
                    //Clicking
                    if (Mouse.current.leftButton.wasPressedThisFrame)
                    {
                        hit.transform.gameObject.GetComponent<PlayerInteractable>().Click(this.gameObject);
                    }
                    //Releasing Click
                    if (Mouse.current.leftButton.wasReleasedThisFrame)
                    {
                        hit.transform.gameObject.GetComponent<PlayerInteractable>().ReleaseClick(this.gameObject);
                    }
                }
            }
        }

        //Handle Switching Between Held Items
        if (Mouse.current.scroll.ReadValue().y > 0)
        {
            inHand++;
            if (inHand >= invSize)
            {
                inHand = 0;
            }
        }
        else if (Mouse.current.scroll.ReadValue().y < 0)
        {
            inHand--;
            if (inHand < 0)
            {
                inHand = invSize - 1;
            }
        }
        for (int i = 0; i < Equipped.Length; i++)
        {
            if (Equipped[i] != null)
            {
                if (inHand == i)
                {
                    Equipped[i].SetActive(true);
                }
                else
                {
                    Equipped[i].SetActive(false);
                }
            }
        }

        if (Equipped[inHand] != null)
        {
            if (Keyboard.current[Key.Q].wasPressedThisFrame)
            {
                //Call the equipped item's drop function.
                Equipped[inHand].GetComponent<PlayerInteractable>().Drop(this.gameObject);
            }
            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                //Call the equipped item's use function.
                Equipped[inHand].GetComponent<PlayerInteractable>().Use(this.gameObject);
            }
            if (Keyboard.current[Key.F].wasPressedThisFrame)
            {
                //Call equipped item's place function.
                Equipped[inHand].GetComponent<PlayerInteractable>().Place(this.gameObject);
            }
        }


    }


    //Gets if there is an empty slot, and if so, output the empty slot.
    public bool CanEquip(out int slot)
    {
        //Debug.Log("Testing Equipability");
        for (int i = 0; i < invSize; i++)
        {
            if (Equipped[i] == null)
            {
                slot = i;
                return true;
            }
        }
        slot = -1;
        return false;
    }

    //Gets if a given slot is empty.
    public bool CanEquip(int slot)
    {
        if (Equipped[slot] == null)
        {
            return true;
        }
        return false;
    }

    //Gets closest hit object using raycast ignoring given tags.
    public static bool RayCastIgnore(Vector3 origin, Vector3 direction, out RaycastHit hit, float distance, HashSet<string> tagsToIgnore = null, HashSet<GameObject> objsToIgnore = null)
    {
        bool hitSomething = false;
        hit = new RaycastHit();
        RaycastHit[] hitList = Physics.RaycastAll(origin, direction, distance);
        float hitDist = float.PositiveInfinity;
        foreach (RaycastHit h in hitList)
        {
            if (!(tagsToIgnore is null) && !tagsToIgnore.Contains(h.transform.gameObject.tag) && !(objsToIgnore is null) && !objsToIgnore.Contains(h.transform.gameObject))
            {
                if (h.distance < hitDist)
                {
                    hitDist = h.distance;
                    hitSomething = true;
                    hit = h;
                }
            }
        }
        return hitSomething;
    }

}
