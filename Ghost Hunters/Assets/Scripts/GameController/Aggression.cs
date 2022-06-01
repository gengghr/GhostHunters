using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Aggression : MonoBehaviour
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
  GameObject[] players;
  private Renderer[] renders;
  public bool ghostAlive;
  public bool renderGhost;

  FlashingLight[] fls;
  // Start is called before the first frame update
  void Start()
  {
    SetDifficultyRate();

    PointLights = GameObject.FindGameObjectsWithTag("PointLights_AggressionLevel");
    doors = GameObject.FindGameObjectsWithTag("Doors_Agg");
    ghost = GameObject.FindGameObjectWithTag("Ghost");
    players = GameObject.FindGameObjectsWithTag("Player");
    ghostAlive = true;
    renderGhost = false;
    RenderGhost(false);
    }

  // Update is called once per frame
  void Update()
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
      ghost.GetComponent<GenericGhostAI>().Aggression = AggressionLevel;
      /*------------------------only for testing purposes, delete this after fully finishing aggression ------------*/

    }
    else
    {

    }
    //if (Keyboard.current[Key.L].wasPressedThisFrame)
    //{
    //  AggressionLevel += 50;//press l let the ghost getting more angry
    //}
    RenderGhost(renderGhost);
  }

  void loweLevel()// 0 - 50
  {
    AggressionLevel += Time.deltaTime * NormalDifficultyRate;
    //ghost.SetActive(false);
    //RenderGhost(false);
    renderGhost = false;
  }

  void MidLevel()//51 - 75
  {
    AggressionLevel += Time.deltaTime * NormalDifficultyRate;
  }
  void HighLevel()//77 - 99
  {
    AggressionLevel += Time.deltaTime * NormalDifficultyRate;
    //ghost.SetActive(true);
    //float n = UnityEngine.Random.Range(0f, 1.0f);
    //if(n <= 0.3f)
    //RenderGhost(true);
    renderGhost = true;
  }
  void Hunting()//let the hunt begin
  {
    if (HuntingTimer < HuntingTime)
    {
      if (!DoOnce)
      {
        foreach (var p in PointLights)//let all lights blink
        {
          p.GetComponent<FlashingLight>().ActiveHuntingLight();
        }
        foreach (var d in doors)//shut down doors
        {
          //Ghost Can Open Doors. No need to shut them anymore.
          //d.GetComponent<DoorOpenScript>().StartHunting();
        }
        foreach (var player in players)//let players know
        {
          player.GetComponent<GetKilled>().SetHunting();
        }


        renderGhost = true;
        //RenderGhost(true);


        //AUDIO
        if(HeartBeat != null){
          HeartBeat.Play();
        }
        DoOnce = true;
      }

      HuntingTimer += Time.deltaTime;
    }
    else
    {
      stopHunt();
    }
  }

  private void SetDifficultyRate()
  {
    if(GameDifficulty == -1){
      NormalDifficultyRate = 0;
    }
    else if (GameDifficulty == 0)
    {
      NormalDifficultyRate = 1f;
    }
    else if (GameDifficulty == 1)
    {
      NormalDifficultyRate = 1.5f;
    }
    else if (GameDifficulty == 2)
    {
      NormalDifficultyRate = 2f;
    }
    else
    {
      NormalDifficultyRate = 40f;
    }
  }

  //Turn Ghost Visible or Invisible
  public void RenderGhost(bool v)
  {
    renders = ghost.GetComponentsInChildren<Renderer>();
    foreach (var r in renders)
    {
      r.enabled = v;
    }
  }

  public void stopHunt()
  {
    AggressionLevel = 0;
    ghost.GetComponent<GenericGhostAI>().Aggression = AggressionLevel;
    HuntingTimer = 0;
    DoOnce = false;
    foreach (var p in PointLights)
    {
      p.GetComponent<FlashingLight>().EndHuntingLight();
    }
    foreach (var d in doors)
    {
      //d.GetComponent<DoorOpenScript>().EndHunting();
    }
    foreach (var player in players)
    {
      player.GetComponent<GetKilled>().EndHunting();
    }
    GameDifficulty = (int)UnityEngine.Random.Range(0, 3.16f);
    SetDifficultyRate();
    HuntingTime = (float)UnityEngine.Random.Range(25f, 45f);

    //AUDIO
    if(HeartBeat != null){
      HeartBeat.Stop();
    }
  }

}
