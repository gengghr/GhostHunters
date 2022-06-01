using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A class for a UV flashlight that reveals invisible things
public class UVRevealer : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(transform.position, transform.up * 100);
        RaycastHit hit;
        bool hitSomething = Physics.Raycast(transform.position, transform.up, out hit, 50);
        // Send a raycast down the direction the revealer is pointing (for the flashlight, this is up)
        if (hitSomething)
        {
            GameObject hitobject = hit.transform.gameObject;
            if (hitobject.CompareTag("UV_Revealable"))
                hitobject.GetComponent<UVRevealable>().UVHit();
            else
                Debug.Log("hit " + hitobject.ToString());
        }
    }
}
