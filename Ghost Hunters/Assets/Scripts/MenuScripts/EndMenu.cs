using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        //Screen.lockCursor = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void GoToMain()
    {
        //SceneManager.UnloadScene("EndGameMenu");
        SceneManager.LoadScene("MainMenu");
        
    }
    public void StartAgain()
    {
        SceneManager.LoadScene("Show");
    }

    public void Exit()
    {
        //if (UnityEditor.EditorApplication.isPlaying)
        //    UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }
}
