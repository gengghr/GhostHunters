using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPlayer : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField]
    private GameObject waiting;
    [SerializeField]
    private GameObject playerData;

    [Header("Data Display")]
    [SerializeField]
    private Text playerNameText;
    [SerializeField]
    private Toggle ready;

    public void UpdateDisplay(LobbyPlayerState lobbyPlayerState)
    {
        playerNameText.text = lobbyPlayerState.PlayerName.ToString();
        ready.isOn = lobbyPlayerState.IsReady;

        waiting.SetActive(false);
        playerData.SetActive(true);
    }

    public void DisableDisplay()
    {
        waiting.SetActive(true);
        playerData.SetActive(false);
    }
}
