using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine.UI;

[RequireComponent(typeof(NetworkObject ))]
[RequireComponent(typeof(NetworkTransform))]
// When the player dies in multiplayer,
// This controller replaces the normal one
public class DeadPlayerControl : NetworkBehaviour
{

    [SerializeField]
    private float speed = 3.5f;

    [SerializeField]
    private const float moveAcceleration = 1.5f; //Acceleration rate

    private CharacterController characterController;

    private Camera cam;

    // horizontal rotation speed
    public float horizontalSpeed = 1f;
    //vertical rotation speed
    public float verticalSpeed = 1f;
    private float xRotation = 0.0f;
    private float yRotation = 0.0f;

    private void Awake()
    {
        // Retrieve variables
        cam = Camera.main;
        characterController = this.GetComponent<CharacterController>();
    }

    private void Start()
    {
        // Only affect the client's player object
        if (IsClient && IsOwner)
        {
            // Convert to a dead player
            Vector3 position = transform.position;
            characterController.detectCollisions = false;
            gameObject.layer = LayerMask.NameToLayer("DeadPlayer");
        }
    }

    private void Update()
    {
        if (IsClient && IsOwner)
        {
            // UI stuff
            if (!PauseMenu.paused && !Journal.open)
            {
                Cursor.lockState = CursorLockMode.Locked;
                ClientMovement();
                ClientCamera();
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
            }
        }

    }

    private void ClientMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        float flight = Input.GetAxis("Flight");


        // player movement
        horizontal *= speed;
        vertical *= speed;
        flight *= speed; // Press Space/Ctrl to fly up/down

        if (Input.GetKey(KeyCode.LeftShift))
        {
            horizontal = horizontal * moveAcceleration;
            vertical = vertical * moveAcceleration;
            flight *= moveAcceleration;
        }

        Vector3 toMove = (characterController.transform.right * horizontal +
            characterController.transform.forward * vertical +
            characterController.transform.up * flight)
            * Time.deltaTime;
        

        characterController.Move(toMove);
    }

    // Camera rotation
    private void ClientCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * horizontalSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * verticalSpeed;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90);

        cam.transform.eulerAngles = new Vector3(xRotation, yRotation, 0.0f);
        transform.eulerAngles = new Vector3(0.0f, yRotation, 0.0f);
    } 

}
