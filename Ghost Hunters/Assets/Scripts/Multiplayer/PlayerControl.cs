using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine.UI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(NetworkTransform))]
public class PlayerControl : NetworkBehaviour
{
  // player states
  public enum PlayerState
  {
    Idle,
    Walk,
    ReverseWalk,
    Run
  }

  // player movement
  [SerializeField]
  private float speed = 1.9f;
  [SerializeField]
  public float Gravity = 9.8f;
  [SerializeField]
  private float velocity = 0;
  [SerializeField]
  private const float moveAcceleration = 1.5f;
  [SerializeField]
  private Vector2 defaultPositionRange = new Vector2(-4, 4);
  private CharacterController characterController;
  [SerializeField]
  private NetworkVariable<PlayerState> networkPlayerState = new NetworkVariable<PlayerState>();
  [SerializeField]
  private GameObject playerPrefab;

  // open door
  [SerializeField]
  private float rayLength = 1.0f;
  private DoorControl raycastedObj;
  private bool doOnce;
  private bool openDoorUI;

  private Animator animator;

  // carmer
  private Camera cam;
  // horizontal rotation speed
  public float horizontalSpeed = 1f;
  //vertical rotation speed
  public float verticalSpeed = 1f;
  private float xRotation = 0.0f;
  private float yRotation = 0.0f;

  public static List<GameObject> playerlist = new List<GameObject>();
  public float horizontalDeadSpace;
  public float verticalDeadSpace;

  private void Awake()
  {
    characterController = GetComponent<CharacterController>();
    animator = GetComponent<Animator>();
    Cursor.lockState = CursorLockMode.Locked;
    cam = Camera.main;
    if (IsServer)
      playerlist.Add(this.gameObject);
  }

  private void Start()
  {
    // set up player position and camera
    if (IsClient && IsOwner)
    {
      //transform.position = new Vector3(Random.Range(defaultPositionRange.x, defaultPositionRange.y), 0,
      //    Random.Range(defaultPositionRange.x, defaultPositionRange.y));

      // first person veiw
      SetLayerRecursively(this.gameObject, "Self");
      Vector3 position = transform.position;
      cam.transform.position = new Vector3(position.x, position.y + 1.7f, position.z);
      cam.transform.parent = transform;
    }
  }

  private void Update()
  {
    if (IsClient && IsOwner)
    {
      // when game not paused or journal opened
      if (!PauseMenu.paused && !Journal.open)
      {
        // update player input
        Cursor.lockState = CursorLockMode.Locked;
        ClientMovement();
        ClientDoor();
        ClientCamera();
      }
      else
      {
        Cursor.lockState = CursorLockMode.None;
      }
    }

    ClientVisuals();
  }

  /*
   * Client movement control
   */
  private void ClientMovement()
  {
    float horizontal = 0;
    float vertical = 0;
    if (Keyboard.current[Key.A].isPressed)
    {
      horizontal -= 1f;
    }
    if (Keyboard.current[Key.D].isPressed)
    {
      horizontal += 1f;
    }
    if (Keyboard.current[Key.W].isPressed)
    {
      vertical += 1f;
    }
    if (Keyboard.current[Key.S].isPressed)
    {
      vertical -= 1f;
    }

    int state = 0;


    // player movement
    horizontal *= speed;
    vertical *= speed;

    // state changes
    if (Keyboard.current[Key.LeftShift].isPressed && (horizontal != 0 || vertical != 0))
    {
      state = 2;
      horizontal = horizontal * moveAcceleration;
      vertical = vertical * moveAcceleration;
    }
    else if (horizontal > 0 || vertical > 0)
    {
      state = 1;
    }
    else if (horizontal < 0 || vertical < 0)
    {
      state = -1;
    }
    else if (horizontal == 0 && vertical == 0)
    {
      state = 0;
    }

    // update new state to server
    if (state == 1)
    {
      UpdatePlayerStateServerRpc(PlayerState.Walk);
    }
    else if (state == 2)
    {
      UpdatePlayerStateServerRpc(PlayerState.Run);
    }
    else if (state == -1)
    {
      UpdatePlayerStateServerRpc(PlayerState.ReverseWalk);
    }
    else
    {
      UpdatePlayerStateServerRpc(PlayerState.Idle);
    }

    // move player
    characterController.Move((characterController.transform.right * horizontal +
        characterController.transform.forward * vertical) * Time.deltaTime);

    // Gravity
    if (characterController.isGrounded)
    {
      velocity = 0;
    }
    else
    {
      velocity -= Gravity * Time.deltaTime;
      characterController.Move(new Vector3(0, velocity, 0));
    }
  }

  /**
   *  Camera control 
   */
  private void ClientCamera()
  {
    float mouseX = Mouse.current.delta.x.ReadValue()*MouseSetting.mouse;
    float mouseY = Mouse.current.delta.y.ReadValue()*MouseSetting.mouse;
    if (Mathf.Abs(mouseX) < horizontalDeadSpace)
    {
      mouseX = 0;
    }
    if (Mathf.Abs(mouseY) < verticalDeadSpace)
    {
      mouseY = 0;
    }

    mouseX *= horizontalSpeed;
    mouseY *= verticalSpeed;

    yRotation += mouseX;
    xRotation -= mouseY;
    xRotation = Mathf.Clamp(xRotation, -90, 90);

    cam.transform.eulerAngles = new Vector3(xRotation, yRotation, 0.0f);
    characterController.transform.eulerAngles = new Vector3(0.0f, yRotation, 0.0f);
  }

  /**
   * player state animation
   */
  private void ClientVisuals()
  {
    if (networkPlayerState.Value == PlayerState.Walk)
    {
      animator.SetFloat("Walk", 1);
    }
    else if (networkPlayerState.Value == PlayerState.ReverseWalk)
    {
      animator.SetFloat("Walk", -1);
    }
    else if (networkPlayerState.Value == PlayerState.Run)
    {
      animator.SetFloat("Walk", 2);
    }
    else
    {
      animator.SetFloat("Walk", 0);
    }
  }

  /**
   *  Client interaction with the door
   */
  private void ClientDoor()
  {
    RaycastHit hit;
    Vector3 fwd = transform.TransformDirection(Vector3.forward);
    int layerMask = LayerMask.GetMask("Door");
    // player raycast a door
    if (Physics.Raycast(transform.position + Vector3.up, fwd, out hit, rayLength, layerMask))
    {
      if (hit.collider.CompareTag("Doors"))
      {
        // active instruction 
        if (!doOnce)
        {
          raycastedObj = hit.collider.gameObject.GetComponent<DoorControl>();
          UIActive(true);
        }
        // set up the UI only once
        doOnce = true;
        openDoorUI = true;

        // open door
        if (Keyboard.current[Key.E].wasPressedThisFrame)
        {
          raycastedObj.UpdateDoorStateServerRpc();
        }
      }
    }
    // player get away from door
    else
    {
      if (openDoorUI)
      {
        UIActive(false);
        doOnce = false;
      }
    }
  }

  /*
   * Set up UI for open and close door
   */
  private void UIActive(bool active)
  {
    if (active && !doOnce)
    {
      DoorUI.active = true;
    }
    else
    {
      openDoorUI = false;
      DoorUI.active = false;
    }
  }

  /*
   * update player state from server
   */
  [ServerRpc]
  public void UpdatePlayerStateServerRpc(PlayerState newState)
  {
    networkPlayerState.Value = newState;
  }


  // When this object is destroyed, remove it from the list
  public override void OnDestroy()
  {
    if (IsServer)
      playerlist.Remove(this.gameObject);
  }

  /*
   *  set layer to the object and all the child
   */
  private void SetLayerRecursively(GameObject obj, string newLayer)
  {
    if (null == obj)
    {
      return;
    }

    obj.layer = LayerMask.NameToLayer(newLayer);

    foreach (Transform child in obj.transform)
    {
      if (null == child)
      {
        continue;
      }
      SetLayerRecursively(child.gameObject, newLayer);
    }
  }
}
