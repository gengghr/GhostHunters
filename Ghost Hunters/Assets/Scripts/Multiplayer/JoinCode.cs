using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinCode : MonoBehaviour
{
    public static JoinCode instance;

    public string joinCode;

    private void Awake()
    {
        instance = this;

        DontDestroyOnLoad(this.gameObject);
    }

    public void SetJoinCode(string code)
    {
        joinCode = code;
    }
}
