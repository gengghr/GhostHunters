using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ServerGameNetPortal : MonoBehaviour
{
    [SerializeField] private int maxPlayers = 4;

    public static ServerGameNetPortal Instance => instance;
    private static ServerGameNetPortal instance;

    private GameNetPortal gameNetPortal;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        gameNetPortal = GetComponent<GameNetPortal>();
        gameNetPortal.OnNetworkReadied += HandleNetworkReadied;

        //NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;

    }

    private void OnDestroy()
    {
        if (gameNetPortal == null) { return; }

        gameNetPortal.OnNetworkReadied -= HandleNetworkReadied;

        if (NetworkManager.Singleton == null) { return; }

        //NetworkManager.Singleton.ConnectionApprovalCallback -= ApprovalCheck;
    }

    // tell all clients load scene
    public void StartGame()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("Multiplayer", LoadSceneMode.Single);
    }


    // Add handshakes and load lobby
    private void HandleNetworkReadied()
    {
        if (!NetworkManager.Singleton.IsServer) { return; }

        gameNetPortal.OnUserDisconnectRequested += HandleUserDisconnectRequested;
        NetworkManager.Singleton.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);

    }


    // Load main menu when client try disconnect
    private void HandleUserDisconnectRequested()
    {

        NetworkManager.Singleton.Shutdown();

        SceneManager.LoadScene("MainMenu");
    }

}
