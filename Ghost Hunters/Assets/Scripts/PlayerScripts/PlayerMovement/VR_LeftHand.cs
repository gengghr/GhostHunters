using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
//the left hand controller
public class VR_LeftHand : MonoBehaviour
{
    //should be the left controller if found
    private InputDevice targetDevice;
    //the  controller model in game
    public GameObject controllerPrefab;
    GameObject spawnedController;
    GameObject spawnedModel;
    bool holdTrigger = false;
    public GameObject player;
    // Start is called before the first frame update\
    // Set up all bound items to be interactable with VR controllers.
    private void Awake()
    {
        List<GameObject> boundItems = new List<GameObject>(GameObject.FindGameObjectsWithTag("BoundItem"));
        foreach (GameObject item in boundItems)
        {
            item.AddComponent<XRGrabInteractable>();
        }
    }
    //check and find the controller
    void Start()
    {
        List<InputDevice> inputDevices = new List<InputDevice>();
        InputDeviceCharacteristics rightController = InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller;
        InputDevices.GetDevicesWithCharacteristics(rightController, inputDevices);
        if (inputDevices.Count > 0)
        {
            targetDevice = inputDevices[0];
        }
        //build the controller model
        spawnedController = Instantiate(controllerPrefab, transform);



    }

    // Update is called once per frame
    void Update()
    {
        //check if the user press the Trigger button
        targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue);
        if (triggerValue > 0.1)
        {
            if (!holdTrigger)
            {
                holdTrigger = true;
                player.GetComponent<VR_PlayerRotation>().setLeftRotation(holdTrigger);
            }
        }
        else
        {
            if (holdTrigger)
            {
                holdTrigger = false;
                player.GetComponent<VR_PlayerRotation>().setLeftRotation(holdTrigger);
            }
        }


    }
}
