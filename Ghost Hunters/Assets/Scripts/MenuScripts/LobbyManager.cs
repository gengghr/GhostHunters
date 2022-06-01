using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using Unity.Services.Authentication;
using Unity.Netcode;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager instance;

    public bool InLobby = false;
    public bool IsHost = false;
    public string lobbyID = null;

    public float timePerHeartbeat = 10f;
    private float timeSinceHeartbeat = 0f;

    private RelayManager relayManager;

    public GameObject[] lobbyListItems;

    // called before Start
    private void Awake()
    {
        // Allow this object to be accessible from anywhere
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        // Login and initialize variables
        InitAsync();
        relayManager = RelayManager.Instance;
        foreach (GameObject item in lobbyListItems)
        {
            item.GetComponentInChildren<Button>().onClick.AddListener(() => JoinLobbyHandle(item.GetComponent<LobbyListItemState>()));
        }
    }

    public async void InitAsync()
    {
        await SignIn();
        // Other methods here, if needed
    }

    // Update is called once per frame
    void Update()
    {
        if (InLobby)
        {
            // If in a lobby, sends messages to keep it alive
            timeSinceHeartbeat += Time.deltaTime;
            if(timeSinceHeartbeat >= timePerHeartbeat)
            {
                timeSinceHeartbeat = 0f;
                Lobbies.Instance.SendHeartbeatPingAsync(lobbyID);
            }
            
        }   
    }

    public async Task SignIn()
    {
        // Anonymously sign in to the Unity Services
        InitializationOptions options = new InitializationOptions().SetEnvironmentName("production");

        //Initialize the Unity Services engine
        await UnityServices.InitializeAsync(options);
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            //If not already logged, log the user in
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    // Async method to create a lobby
    public async Task CreateLobbyAsync()
    {
        // Retrieve the join code
        DataObject data = new DataObject(DataObject.VisibilityOptions.Member, JoinCode.instance.joinCode);
        // Create the lobby and save the join code to it
        CreateLobbyOptions options = new CreateLobbyOptions();
        options.Data = new Dictionary<string, DataObject>();
        options.Data.Add("JoinCode", data);
        string name = PlayerPrefs.GetString("PlayerName", "Lobby");
        if(name == null || name == "")
        {
            name = "Lobby";
        }
        Lobby host = await Lobbies.Instance.CreateLobbyAsync(name, 4, options);
        lobbyID = host.Id;
        InLobby = true;
        IsHost = true;
    }

    public async Task CloseLobbyAsync()
    {
        await Lobbies.Instance.DeleteLobbyAsync(lobbyID);
        IsHost = false;
        InLobby = false;
    }

    // Unity Buttons don't like using async methods,
    // So here is a synchronous method that calls it
    public void JoinLobbyHandle(LobbyListItemState state)
    {
        JoinLobbyAsync(state);
    }

    // Async method to join a lobby
    public async Task JoinLobbyAsync(LobbyListItemState state)
    {
        // Retrieve the lobby id and join it
        Lobby l = await Lobbies.Instance.JoinLobbyByIdAsync(state.lobbyID, default);
        InLobby = true;
        // Retrieve the Relay join code from the lobby and join the room
        string joinCode = l.Data["JoinCode"].Value;
        JoinCode.instance.SetJoinCode(joinCode);
        MainMenu.instance.OnClientClickedAsync();
    }

    public async Task LeaveLobbyAsync()
    {
        string playerId = AuthenticationService.Instance.PlayerId;
        await Lobbies.Instance.RemovePlayerAsync(lobbyID, playerId);
        InLobby = false;
    }

    public void RefreshLobbyListHandle()
    {
        RefreshLobbyListAsync();
    }

    // Async method to get a list of lobbies and create a UI list for them
    public async Task RefreshLobbyListAsync()
    {
        // Remove the previous list
        foreach (GameObject item in lobbyListItems)
        {
            item.SetActive(false);
        }
        // Retrieve a list of new lobbies
        QueryLobbiesOptions options = new QueryLobbiesOptions();
        options.Count = 10;
        QueryResponse response = await Lobbies.Instance.QueryLobbiesAsync(options);
        List<Lobby> lobbies = response.Results;
        // Create the UI items
        for (int i = 0; i < lobbies.Count; i++)
        {
            Lobby l = lobbies[i];
            GameObject item = lobbyListItems[i];
            LobbyListItemState state = item.GetComponent<LobbyListItemState>();
            state.SetFields(l.Name, l.Players.Count);
            state.lobbyID = l.Id;
            item.SetActive(true);
        }
        
    }

    // Automatically leave the lobby when this object is destroyed.
    private void OnDestroy()
    {
        if (IsHost)
        {
            Task t = CloseLobbyAsync();
            t.Wait(1000);
        }
        else if (InLobby)
        {
            Task t = LeaveLobbyAsync();
            t.Wait(1000);
        }
    }
}
