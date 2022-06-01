using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyListItemState : MonoBehaviour
{
    // State of an item in the lobby list
    public string lobbyID = "";
    public string lobbyName = "";
    public int pop = 0;
    public string gameJoinCode = "";

    public Text nameField;
    public Text popField;

    // Sets the text fields of the item
    public void SetFields(string name, int pop)
    {
        nameField.text = name;
        popField.text = pop.ToString() + "/4";
        lobbyName = name;
        this.pop = pop;
    }
    
}
