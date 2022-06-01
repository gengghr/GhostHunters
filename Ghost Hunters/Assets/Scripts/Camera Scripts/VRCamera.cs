using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRCamera : MonoBehaviour
{
    //Set the main camera
    public Camera cm;
    CharacterController characterController;
    // Start is called before the first frame update
    void Start()
    {
        //set up the character controller
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    // Using camera to rotate the player
    void Update()
    {
        var rotation = cm.transform.rotation.eulerAngles;
        rotation.x = 0;
        rotation.z = 0;
        characterController.transform.eulerAngles = rotation;
    }
}
