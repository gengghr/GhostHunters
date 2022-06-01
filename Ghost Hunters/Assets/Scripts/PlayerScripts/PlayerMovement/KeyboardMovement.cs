using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeyboardMovement : MonoBehaviour
{
  CharacterController characterController;
  public float MovementSpeed = 2;
  public float Gravity = 0.2f;
  private float velocity = 0;
  private const float moveAcceleration = 1.2f; //Acceleration rate

  public Animator _animator;
  // animation IDs
  private int _animIDSpeed;
  private int _animIDGrounded;
  private int _animIDJump;
  private int _animIDFreeFall;
  private int _animIDMotionSpeed;

  private int montion;
  private float animSpeed;

  bool lshift = true;
  // Start is called before the first frame update
  void Start()
  {
    characterController = GetComponent<CharacterController>();
    AssignAnimationIDs();
  }

  // Update is called once per frame
  void Update()
  {
    float horizontal = 0;
    float vertical = 0;
    if (Keyboard.current[Key.A].isPressed){
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

    lshift = Keyboard.current[Key.LeftShift].isPressed;

    Move(horizontal, vertical, lshift);

  }

    //move the player to the given x,y direction
    private void Move(float horizontal, float vertical, bool lshift)
  {
    if (!PauseMenu.paused && !Journal.open)
    {
      // player movement
      horizontal *= MovementSpeed;
      vertical *= MovementSpeed;
      animSpeed = MovementSpeed;

      //left shift to move faster
      if (lshift)
      {
        horizontal = horizontal * moveAcceleration;
        vertical = vertical * moveAcceleration;
        //montion = 6;
        animSpeed = MovementSpeed * moveAcceleration;
      }
      if (horizontal == 0 && vertical == 0)
      {
        montion = 0;
        animSpeed = 0;
      }
      else if (!lshift)
      {
        montion = 1;
      }
      characterController.Move((characterController.transform.right * horizontal + characterController.transform.forward * vertical) * Time.deltaTime);

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
      //_animationBlend = Mathf.Lerp(_animationBlend, MovementSpeed, Time.deltaTime);
      _animator.SetFloat(_animIDSpeed, animSpeed);
      _animator.SetFloat(_animIDMotionSpeed, montion);
    }
  }
  //set up the addresses
  private void AssignAnimationIDs()
  {
    _animIDSpeed = Animator.StringToHash("Speed");
    _animIDGrounded = Animator.StringToHash("Grounded");
    _animIDJump = Animator.StringToHash("Jump");
    _animIDFreeFall = Animator.StringToHash("FreeFall");
    _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
  }



}
