using System;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameNetPortal : MonoBehaviour
{
    public static GameNetPortal Instance => instance;
    private static GameNetPortal instance;

    public event Action OnNetworkReadied;

    public event Action<ulong, int> OnClientSceneChanged;

    public event Action OnUserDisconnectRequested;

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
        NetworkManager.Singleton.OnServerStarted += HandleNetworkReady;
        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnServerStarted -= HandleNetworkReady;
            NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;

            if (NetworkManager.Singleton.CustomMessagingManager == null) { return; }

        }
    }

    // start as a host
    public async Task StartHostAsync()
    {
        // set up relay server
        if (RelayManager.Instance.IsRelayEnabled)
            await RelayManager.Instance.SetupRelay();
        NetworkManager.Singleton.StartHost();

        // Set up the lobby
        await LobbyManager.instance.CreateLobbyAsync();
    }

    // client request disconnect
    public void RequestDisconnect()
    {
        OnUserDisconnectRequested?.Invoke();
    }

    // client connected
    private void HandleClientConnected(ulong clientId)
    {
        if (clientId != NetworkManager.Singleton.LocalClientId) { return; }

        //NetworkManager.Singleton.NetworkConfig.ConnectionApproval = true; 
        HandleNetworkReady();
    }

    // call network ready
    private void HandleNetworkReady()
    {
        OnNetworkReadied?.Invoke();
    }

    public void clearALL()
    {
        Destroy(GameNetPortal.instance);
        Destroy(ServerGameNetPortal.Instance);
        Destroy(ClientGameNetPortal.Instance);
    }
}
