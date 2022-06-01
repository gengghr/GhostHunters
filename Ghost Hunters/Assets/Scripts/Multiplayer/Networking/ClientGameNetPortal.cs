using System;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(GameNetPortal))]
public class ClientGameNetPortal : MonoBehaviour
{
    public static ClientGameNetPortal Instance => instance;
    private static ClientGameNetPortal instance;

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

        // handshakes
        gameNetPortal.OnNetworkReadied += HandleNetworkReadied;
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;
    }

    private void OnDestroy()
    {
        if (gameNetPortal == null) { return; }

        gameNetPortal.OnNetworkReadied -= HandleNetworkReadied;
        if (NetworkManager.Singleton == null) { return; }

        NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnect;
    }

    public async Task StartClientAsync(string joincode)
    {
        // maker sure relay server is up
        if (RelayManager.Instance.IsRelayEnabled)
        {
            await RelayManager.Instance.JoinRelay(joincode);
            // connected to the host
            NetworkManager.Singleton.StartClient();
        }
    }

    // add disconnect handshake for each client
    private void HandleNetworkReadied()
    {
        if (!NetworkManager.Singleton.IsClient) { return; }

        if (!NetworkManager.Singleton.IsHost)
        {
            gameNetPortal.OnUserDisconnectRequested += HandleUserDisconnectRequested;
        }
    }

    // go to main menu and close network manager
    private void HandleUserDisconnectRequested()
    {
        NetworkManager.Singleton.Shutdown();

        HandleClientDisconnect(NetworkManager.Singleton.LocalClientId);

        SceneManager.LoadScene("MainMenu");
    }

    // when client disconnect
    private void HandleClientDisconnect(ulong clientId)
    {
        if (!NetworkManager.Singleton.IsConnectedClient && !NetworkManager.Singleton.IsHost)
        {
            gameNetPortal.OnUserDisconnectRequested -= HandleUserDisconnectRequested;

            if (SceneManager.GetActiveScene().name != "MainMenu")
            {

                SceneManager.LoadScene("MainMenu");
            }
        }
    }
}
