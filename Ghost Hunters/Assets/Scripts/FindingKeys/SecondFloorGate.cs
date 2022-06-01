using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondFloorGate : MonoBehaviour
{
    public bool key1_found;
    public bool key2_found;
    public bool key3_found;
    private Animator animator;
    private GameObject player;
    private Transform playerTrans;
    private float distance;//distance between the object and the player
    private const float operation_distance = 3;//checking distance for actions.
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");//get tag: player objects
        playerTrans = player.transform;//player vector3d
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        distance = Vector3.Distance(playerTrans.position, transform.position);
        if (distance < operation_distance && key1_found && key2_found && key3_found && Input.GetKeyUp(KeyCode.E))
        {
            animator.SetBool("open", true); 
        }
    }
}
