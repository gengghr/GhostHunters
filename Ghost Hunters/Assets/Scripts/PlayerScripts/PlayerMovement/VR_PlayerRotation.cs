using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VR_PlayerRotation : MonoBehaviour
{
    private CharacterController characterController;
    private bool rightRotate;
    private bool leftRotate;
    private float rotationPerSecond = 60;
    private Vector3 currentEulerAngles;
    
    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        rightRotate = false;
    }

    // Update is called once per frame
    // do the rotation on given EulerAngle
    void Update()
    {
        if(rightRotate)
        {
            currentEulerAngles += new Vector3(0, 1, 0) * Time.deltaTime * rotationPerSecond;
            characterController.transform.eulerAngles = currentEulerAngles;
        }
        else if (leftRotate)
        {
            currentEulerAngles += new Vector3(0, -1, 0) * Time.deltaTime * rotationPerSecond;
            characterController.transform.eulerAngles = currentEulerAngles;
        }
    }
    //API for right roatation
    public void setRightRotation(bool b)
    {
        rightRotate = b;
    }
    //API for left rotation
    public void setLeftRotation(bool b)
    {
        leftRotate = b;
    }
}
