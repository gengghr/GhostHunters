using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : NetworkBehaviour
{
    [Header("References")]
    [SerializeField]
    private LobbyPlayer[] lobbyPlayersUI;
    [SerializeField]
    private Button startGameButton;
    [SerializeField]
    private Text JoinCodeText;

    private NetworkList<LobbyPlayerState> lobbyPlayers;

    private void Awake()
    {
        lobbyPlayers = new NetworkList<LobbyPlayerState>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            lobbyPlayers.OnListChanged += HandleLobbyPlayersStateChanged;
        }

        if (IsServer)
        {
            //JoinCodeText.gameObject.SetActive(true);
            JoinCodeText.text = "Join Code: " + JoinCode.instance.joinCode;


            // Handshake for client connect and disconnect
            NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;

            // add client to list
            foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
            {
                HandleClientConnected(client.ClientId);
            }
        }
    }

    // When object Destoryed.
    public override void OnDestroy()
    {
        base.OnDestroy();

        LoadingScreen.instance.activeLoading(true);

        lobbyPlayers.OnListChanged -= HandleLobbyPlayersStateChanged;

        if (NetworkManager.Singleton)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnect;
        }
    }

    // check if all the player ready
    private bool IsEveryoneReady()
    {
        if (lobbyPlayers.Count < 1)
        {
            return false;
        }

        foreach (var player in lobbyPlayers)
        {
            if (!player.IsReady)
            {
                return false;
            }
        }

        return true;
    }

    // when player connected
    private void HandleClientConnected(ulong clientId)
    {
        lobbyPlayers.Add(new LobbyPlayerState(clientId, "Player " + clientId, false));
    }

    // when player disconnected
    private void HandleClientDisconnect(ulong clientId)
    {
        for (int i = 0; i < lobbyPlayers.Count; i++)
        {
            if (lobbyPlayers[i].ClientId == clientId)
            {
                lobbyPlayers.RemoveAt(i);
                break;
            }
        }
    }

    // let server update client
    [ServerRpc(RequireOwnership = false)]
    private void ToggleReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        for (int i = 0; i < lobbyPlayers.Count; i++)
        {
            if (lobbyPlayers[i].ClientId == serverRpcParams.Receive.SenderClientId)
            {
                lobbyPlayers[i] = new LobbyPlayerState(
                    lobbyPlayers[i].ClientId,
                    lobbyPlayers[i].PlayerName,
                    !lobbyPlayers[i].IsReady
                );
            }
        }
    }

    // tell all the client game started
    [ServerRpc(RequireOwnership = false)]
    private void StartGameServerRpc(ServerRpcParams serverRpcParams = default)
    {
        if (serverRpcParams.Receive.SenderClientId != NetworkManager.Singleton.LocalClientId) { return; }

        if (!IsEveryoneReady()) { return; }

        ServerGameNetPortal.Instance.StartGame();
    }

    // client leave
    public void OnLeaveClicked()
    {
        GameNetPortal.Instance.RequestDisconnect();
        //GameNetPortal.Instance.clearALL();
    }

    // client ready
    public void OnReadyClicked()
    {
        ToggleReadyServerRpc();
    }

    // host start game
    public void OnStartGameClicked()
    {
        StartGameServerRpc();
    }

    // when player get ready
    private void HandleLobbyPlayersStateChanged(NetworkListEvent<LobbyPlayerState> lobbyState)
    {
        // update list
        for (int i = 0; i < lobbyPlayersUI.Length; i++)
        {
            if (lobbyPlayers.Count > i)
            {
                lobbyPlayersUI[i].UpdateDisplay(lobbyPlayers[i]);
            }
            else
            {
                lobbyPlayersUI[i].DisableDisplay();
            }
        }

        // check if host can start game
        if (IsHost)
        {
            startGameButton.gameObject.SetActive(IsEveryoneReady());
        }
    }
}