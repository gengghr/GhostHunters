using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
//the right hand controller
public class VR_RightHand : MonoBehaviour
{
    
    public GameObject player;
    private InputDevice targetDevice;
    public GameObject controllerPrefab;
    GameObject spawnedController;
    bool holdTrigger = false;
    // Start is called before the first frame update
    // check and find the controller
    void Start()
    {
        List<InputDevice> inputDevices = new List<InputDevice>();
        InputDeviceCharacteristics rightController = InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller;
        InputDevices.GetDevicesWithCharacteristics(rightController, inputDevices);
        if(inputDevices.Count>0)
        {
            targetDevice = inputDevices[0];
        }
        spawnedController = Instantiate(controllerPrefab, transform);

    }

    // Update is called once per frame
    void Update()
    {

        //check if the user press the Trigger button
        targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue);
        if(triggerValue>0.1)
        {
            if(!holdTrigger)
            {
                holdTrigger = true;
                player.GetComponent<VR_PlayerRotation>().setRightRotation(holdTrigger);
            }
        }
        else
        {
            if (holdTrigger)
            {
                holdTrigger = false;
                player.GetComponent<VR_PlayerRotation>().setRightRotation(holdTrigger);
            }
        }
 
        //Debug.Log(player.name);
        //player.GetComponent<VR_PlayerRotation>().setRotation(holdTrigger);
       



    }
}
