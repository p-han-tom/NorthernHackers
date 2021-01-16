using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MLAPI;
using MLAPI.Transports.UNET;
using MLAPI.Messaging;

public class UIManager : NetworkedBehaviour
{
    GameObject networkingMenu;
    GameObject gameMenu;
    GameObject startButton;
    TMP_InputField nameField;
    TMP_InputField ipField;
    StageManager sm;
    //clientId, playername
    Dictionary<ulong, string> playerNames;
    Dictionary<string, int> woodTracker;
    Dictionary<string, int> pointTracker;
    void Start()
    {
        sm = GameObject.Find("Stage").GetComponent<StageManager>();
        networkingMenu = transform.Find("Networking Menu").gameObject;
        gameMenu = transform.Find("Game Menu").gameObject;
        startButton = gameMenu.transform.Find("Go").gameObject;
        startButton.SetActive(false);
        nameField = networkingMenu.transform.Find("Name").GetComponentInChildren<TMP_InputField>();
        ipField = networkingMenu.transform.Find("IP Address").GetComponentInChildren<TMP_InputField>();
        playerNames = new Dictionary<ulong, string>();
        woodTracker = new Dictionary<string, int>();
        pointTracker = new Dictionary<string, int>();
    }

    public void HostGame()
    {
        NetworkingManager.Singleton.StartHost();
        networkingMenu.SetActive(false);
        sm.initiate = true;
        startButton.SetActive(true);
        InitiatePlayer(NetworkingManager.Singleton.LocalClientId);
    }
    public void JoinGame()
    {
        string ip = ipField.text;
        if (ip.Trim() == "")
        {
            NetworkingManager.Singleton.GetComponent<UnetTransport>().ConnectAddress = "127.0.0.1";
        }
        else
        {
            NetworkingManager.Singleton.GetComponent<UnetTransport>().ConnectAddress = ip;
        }
        NetworkingManager.Singleton.StartClient();
        //Initiate player when client is connected to host
        NetworkingManager.Singleton.OnClientConnectedCallback += InitiatePlayer;
        networkingMenu.SetActive(false);
    }
    // Set player name, and add to score array
    void InitiatePlayer(ulong clientId)
    {
        string name = nameField.text;
        if (name.Trim() == "")
        {
            name = "Player " + NetworkingManager.Singleton.LocalClientId.ToString();
        }

        // if (!IsServer) 
        InvokeServerRpc(ClientInitiatePlayer, clientId, name);
        // else
        // {
        //     playerNames.Add(clientId, name);
        //     woodTracker.Add(name, 0);
        //     pointTracker.Add(name, 0);
        // }
    }
    [ServerRPC]
    void ClientInitiatePlayer(ulong clientId, string name)
    {
        playerNames.Add(clientId, name);
        woodTracker.Add(name, 0);
        pointTracker.Add(name, 0);
    }

    // Called from server
    public void AddWood(ulong clientId, int wood)
    {
        string playerName = GetName(clientId);
        woodTracker[playerName] = woodTracker[playerName] + wood;
        Debug.Log(GetName(clientId) + " has " + woodTracker[playerName] + " wood.");

    }
    // Called from server TODO: add call for this
    public void AddPoint(ulong clientId)
    {
        string playerName = GetName(clientId);
        pointTracker[playerName] = pointTracker[playerName] + 1;
        Debug.Log(GetName(clientId) + " has " + woodTracker[playerName] + " points.");
    }
    string GetName(ulong clientId)
    {
        string playerName;
        if (playerNames.TryGetValue(clientId, out playerName))
        {
            return playerName;
        }
        return null;
    }
}
