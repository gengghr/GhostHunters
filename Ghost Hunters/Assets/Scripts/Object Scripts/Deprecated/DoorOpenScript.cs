using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
public class DoorOpenScript : MonoBehaviour
{
    private Animator animator;
    private GameObject player;
    private Transform playerTrans;
    private float distance;//distance between the object and the player
    private const float operation_distance = 3;//checking distance for actions.
    public bool hunting_time = false;
    [SerializeField]
    private InputDevice targetDevice;

    // Start is called before the first frame update
    void Start()
    {
        //animator = go.GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");//get tag: player objects
        playerTrans = player.transform;//player vector3d
        animator = GetComponent<Animator>();

        List<InputDevice> inputDevices = new List<InputDevice>();
        InputDeviceCharacteristics rightController = InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller;
        InputDevices.GetDevicesWithCharacteristics(rightController, inputDevices);
        if (inputDevices.Count > 0)
        {
            targetDevice = inputDevices[0];
        }
    }

    private void OnTriggerEnter(Collider other)
    {
 
        //animator.SetBool("open",true);
    }

    // Update is called once per frame
    void Update()
    {
        targetDevice.TryGetFeatureValue(CommonUsages.gripButton, out bool pressed);
        Debug.Log("grip!");
        distance = Vector3.Distance(playerTrans.position, transform.position);
        if (!hunting_time &&  pressed && distance < operation_distance && !animator.GetBool("open"))//if door is closed, open it.
        {
            animator.SetBool("open", true);
        }
        else if (!hunting_time &&  pressed && distance < operation_distance && animator.GetBool("open"))//if door is opened, close it.
        {
            animator.SetBool("open", false);
        }

    }

    public void StartHunting()
    {
        hunting_time=true;
        animator.SetBool("open", false);
    }
    public void EndHunting()
    {
        hunting_time = false;
    }
}
