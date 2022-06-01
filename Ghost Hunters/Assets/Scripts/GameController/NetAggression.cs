using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Samples;
using UnityEngine.InputSystem;

[RequireComponent(typeof(NetworkObject))]
public class NetAggression : NetworkBehaviour
{
    public float AggressionLevel;
    public int GameDifficulty = 1;//-1==Ghost Never Attacks, 0== easy, 1== mid, 2==hard, >3 == you die
    float NormalDifficultyRate;
    float HuntingTime = 30f;
    float HuntingTimer = 0;
    bool DoOnce = false;

    public AudioSource HeartBeat;

    public static bool ImAngry = false;

    GameObject[] PointLights;
    GameObject[] doors;
    public GameObject ghost;
    public List<GameObject> players;
    private Renderer[] renders;
    public bool ghostAlive;
    public bool renderGhost;

    FlashingLight[] fls;
    // Start is called before the first frame update
    void Start()
    {
        // Setup variables
        AggressionLevel = 0;
        SetDifficultyRate();

        PointLights = GameObject.FindGameObjectsWithTag("PointLights_AggressionLevel");
        doors = GameObject.FindGameObjectsWithTag("Doors_Agg");
        ghost = GameObject.FindGameObjectWithTag("Ghost");
        players = PlayerControl.playerlist;
        ghostAlive = true;
        renderGhost = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Agression is controlled by the server
        if (IsServer && ghostAlive)
        {
            if (ghostAlive)
            {
                if (ImAngry)
                {
                    AggressionLevel = 100;
                }
                if (AggressionLevel <= 50)
                {
                    loweLevel();
                }
                else if (AggressionLevel <= 75)
                {
                    MidLevel();
                }
                else if (AggressionLevel < 100)
                {
                    HighLevel();
                }
                else if (AggressionLevel >= 100)
                {
                    Hunting();
                }
                ghost.GetComponent<NetGenericGhostBehavior>().Aggression = AggressionLevel;
                /*------------------------only for testing purposes, delete this after fully finishing aggression ------------*/
                if (Keyboard.current[Key.L].wasPressedThisFrame)
                {
                    AggressionLevel += 50;//press l let the ghost getting more angry
                }
            }
            
        }
    }

    void loweLevel()// 0 - 50
    {
        AggressionLevel += Time.deltaTime * NormalDifficultyRate;
        //ghost.SetActive(false);
        RenderGhostClientRpc(false);
    }

    void MidLevel()//51 - 75
    {
        AggressionLevel += Time.deltaTime * NormalDifficultyRate * 1.9f;
        RenderGhostClientRpc(false);
    }
    void HighLevel()//77 - 99
    {
        AggressionLevel += Time.deltaTime * NormalDifficultyRate * 2.5f;
        //ghost.SetActive(true);
        //float n = UnityEngine.Random.Range(0f, 1.0f);
        //if(n <= 0.3f)
        RenderGhostClientRpc(true);
    }
    void Hunting()//let the hunt begin
    {
        if(HuntingTimer < HuntingTime)
        {
            if (!DoOnce)
            {
                StartHuntingClientRpc();
                foreach (var player in PlayerControl.playerlist)//let player objects know
                {
                    player.GetComponent<NetGetKilled>().SetHunting();
                }
                DoOnce = true;
            }
            RenderGhostClientRpc(true);
            HuntingTimer += Time.deltaTime;
        }
        else
        {
            stopHunt();
        }
    }

    public void stopHunt()
    {
        // Reset aggression values
        AggressionLevel = 0;
        HuntingTimer = 0;
        DoOnce = false;
        // Let the players know
        EndHuntingClientRpc();
        foreach (var player in PlayerControl.playerlist)
        {
            player.GetComponent<NetGetKilled>().EndHunting();
        }
        // Start the cycle again with a random difficulty
        GameDifficulty = (int)UnityEngine.Random.Range(-1, 4);
        SetDifficultyRate();
        HuntingTime = (float)UnityEngine.Random.Range(25f, 45f);
    }

    // Start client-side actions for the hunting phase
    [ClientRpc]
    private void StartHuntingClientRpc()
    {
        foreach (var p in PointLights)//let all lights blink
        {
            p.GetComponent<FlashingLight>().ActiveHuntingLight();
        }
        foreach (var d in doors)//shut down doors
        {
            d.GetComponent<DoorOpenScript>().StartHunting();
        }
        
        //AUDIO
        HeartBeat.Play();
    }

    // End client-side actions of the hunting phase
    [ClientRpc]
    public void EndHuntingClientRpc()
    {
        foreach (var p in PointLights)
        {
            p.GetComponent<FlashingLight>().EndHuntingLight();
        }
        foreach (var d in doors)
        {
            d.GetComponent<DoorOpenScript>().EndHunting();
        }
        
        //AUDIO
        HeartBeat.Stop();
    }

    private void SetDifficultyRate()
    {
        if(GameDifficulty == -1)
        {
            NormalDifficultyRate = 0;
        }
        else if (GameDifficulty == 0)
        {
            NormalDifficultyRate = 1;
        }
        else if (GameDifficulty == 1)
        {
            NormalDifficultyRate = 1.5f;
        }
        else if (GameDifficulty == 2)
        {
            NormalDifficultyRate = 2.5f;
        }
        else
        {
            NormalDifficultyRate = 100f;
        }
    }

    //Turn Ghost Visible or Invisible on the Client
    [ClientRpc]
    public void RenderGhostClientRpc(bool v)
    {
        Renderer[] ghostRenders = ghost.transform.GetChild(0).GetChild(0).GetComponentsInChildren<Renderer>();
        for(int i = 0; i < ghostRenders.Length; i++)
        {
            ghostRenders[i].enabled = v;
        }
    }

    public void ghostDead()
    {
        ghostAlive = false;
    }

}
