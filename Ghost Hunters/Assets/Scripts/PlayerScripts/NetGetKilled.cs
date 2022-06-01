using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using Unity.Netcode.Samples;

// Detects if the ghost kills a player
[RequireComponent(typeof(NetworkObject))]
public class NetGetKilled : NetworkBehaviour
{
    private bool IsHunting;
    private bool IsAlive;
    private GameObject ghost;
    private float distance;
    private const float catchDis = 1.1f;

    // Start is called before the first frame update
    void Start()
    {
        // Setup variables
        IsHunting = false;
        IsAlive = true;
        ghost = GameObject.FindGameObjectWithTag("Ghost");
    }

    // Update is called once per frame
    void Update()
    {
        // Calculations are done server-side
        if (IsServer)
        {
            distance = Vector3.Distance(ghost.transform.position, transform.position);
            if (IsAlive && IsHunting && distance < catchDis)
            {
                // Notify the player that they are dead
                IsAlive = false;
                PlayerControl.playerlist.Remove(this.gameObject);
                DieClientRpc();

                // Check if all the players are dead
                if(PlayerControl.playerlist.Count == 0)
                {
                    GameOverClientRpc();
                }
            }

        }
        
    }

    public void SetHunting()
    {
        IsHunting=true;
    }
    public void EndHunting()
    {
        IsHunting=false;
    }

    // Client-side operations when a character dies
    [ClientRpc]
    private void DieClientRpc()
    {
        if(IsClient && IsOwner)
        {
            IsAlive = false;
            GetComponent<PlayerControl>().enabled = false;
            GetComponent<DeadPlayerControl>().enabled = true;
        }
        else
        {
            RenderPlayer(false);
        }
    }

    // Set the player invisible or visible
    private void RenderPlayer(bool v)
    {
        if (v)
            SetLayerRecursively(this.gameObject, "Default");
        else
            SetLayerRecursively(this.gameObject, "DeadPlayer");
    }

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

    // Send all the players to the Game Over screen
    [ClientRpc]
    private void GameOverClientRpc()
    {
        SceneManager.LoadScene("EndGameMenu");
    }
}
