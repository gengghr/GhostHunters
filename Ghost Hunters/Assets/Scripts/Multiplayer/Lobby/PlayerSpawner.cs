using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Networking.Transport;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField]
    private GameObject playerPrefab;

    // Start is called before the first frame update
    void Start()
    {
        LoadingScreen.instance.activeLoading(false);
        SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId);
    }

    // position for each player
    public Vector3 getPosition(ulong clientID)
    {
        Vector3 position = Vector3.zero;
        switch (clientID)
        {
            case 0:
                position = new Vector3(0, 0.2f, 0);
                break;
            case 1:
                position = new Vector3(4, 0.2f, 0);
                break;
            case 2:
                position = new Vector3(4, 0.2f, 0);
                break;
            case 3:
                position = new Vector3(4, 0.2f, 4);
                break;

        }
        return position;
    }

    // call server to spawn a player prefab
    [ServerRpc(RequireOwnership = false)]
    private void SpawnPlayerServerRpc(ulong clientID)
    {
        GameObject player = Instantiate(playerPrefab, getPosition(clientID), Quaternion.identity);
        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientID);

        ulong objectID = player.GetComponent<NetworkObject>().NetworkObjectId;
        SpawnClientRpc(objectID);
    }

    // update prefab information on each client
    [ClientRpc]
    private void SpawnClientRpc(ulong objectID)
    {
        NetworkObject player = NetworkManager.Singleton.SpawnManager.SpawnedObjects[objectID];
    }
}
