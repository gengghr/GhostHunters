using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using System;

public class UIManager : Singletons<UIManager>
{
    [SerializeField]
    private Button startServerButton;
    [SerializeField]
    private Button startHostButton;
    [SerializeField]
    private Button startClientButton;

    [SerializeField]
    private InputField joinCodeInput; 

    public GameObject ipMenue;
    public GameObject wrongInput;
    public GameObject HUD;

    private void Awake()
    {
        Cursor.visible = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        startHostButton?.onClick.AddListener(startHostAsync);
        startServerButton?.onClick.AddListener(startServer);
        startClientButton?.onClick.AddListener(startClientAsync);
        PauseMenu.paused = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*
     * start as host
     */
    public async void startHostAsync()
    {
        // set up relay server
        if (RelayManager.Instance.IsRelayEnabled)
            await RelayManager.Instance.SetupRelay();
        if (NetworkManager.Singleton.StartHost())
        {
            Debug.Log("Server started...");
        }
        else
        {
            Debug.Log("Host can't be started");
        }
    }

    /*
     * start as server
     */
    public void startServer()
    {
        if (NetworkManager.Singleton.StartServer())
        {
            Debug.Log("Server Started");
        }
        else
        {
            Debug.Log("Server can't be started");
        }

    }

    /*
     * start as client
     */
    public async void startClientAsync()
    {
        // maker sure relay server is up
        if (RelayManager.Instance.IsRelayEnabled)
        {
            try
            {
                await RelayManager.Instance.JoinRelay(joinCodeInput.text);
                // connected to the host
                if (NetworkManager.Singleton.StartClient())
                {
                    PauseMenu.paused = false;
                    PauseMenu.started = true;
                    Cursor.lockState = CursorLockMode.Locked;
                    HUD.SetActive(true);

                    Debug.Log("Client connected");
                }
                else
                {
                    Debug.Log("Client can't be started");
                }
            }
            // wrong join code
            catch (Exception e)
            {
                wrongInput.SetActive(true);
                Debug.Log(e.Data);
            }

        }

    }
}
