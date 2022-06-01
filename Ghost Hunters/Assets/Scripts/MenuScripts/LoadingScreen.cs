using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen instance;
    [SerializeField]
    private GameObject loading;

    private void Awake()
    {
        instance = this;

        DontDestroyOnLoad(this.gameObject);
    }

    // loading screen control
    public void activeLoading(bool b)
    {
        loading.SetActive(b);
    }

}
