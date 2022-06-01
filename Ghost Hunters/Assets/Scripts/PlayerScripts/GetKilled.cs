using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GetKilled : MonoBehaviour
{
    private bool IsHunting;
    private bool IsAlive;
    private GameObject ghost;
    private float distance;
    private const float catchDis = 1.1f;
    // Start is called before the first frame update
    void Start()
    {
        IsHunting = false;
        IsAlive = true;
        ghost = GameObject.FindGameObjectWithTag("Ghost");
    }

    // Update is called once per frame
    void Update()
    {
        distance = Vector3.Distance(ghost.transform.position, transform.position);
        if(IsHunting && distance < catchDis)
        {
            IsAlive = false;
            
            SceneManager.LoadScene("EndGameMenu");
            
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
}
