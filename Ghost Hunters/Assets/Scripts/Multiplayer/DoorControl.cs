using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(NetworkTransform))]
public class DoorControl : NetworkBehaviour
{
    public enum DoorState
    {
        close,
        open
    }

    public NetworkVariable<DoorState> networkDoorState = new NetworkVariable<DoorState>();

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // update Door state and animation
    private void Update()
    {
        DoorVisuals();
    }

    // door animation
    private void DoorVisuals()
    {
        if (networkDoorState.Value == DoorState.open)
        {
            animator.SetBool("open", true);
        }
        else
        {
            animator.SetBool("open", false);
        }
    }

    // update door state from server
    [ServerRpc (RequireOwnership = false)]
    public void UpdateDoorStateServerRpc()
    {
        if(networkDoorState.Value == DoorState.close)
        {
            networkDoorState.Value = DoorState.open;
        }
        else
        {
            networkDoorState.Value = DoorState.close;
        }

    }
}
