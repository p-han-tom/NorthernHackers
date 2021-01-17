using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using MLAPI;
using MLAPI.Transports.UNET;
using MLAPI.Messaging;
using MLAPI.NetworkedVar;
using UnityEngine.SceneManagement;

public class UIManager : NetworkedBehaviour
{
    GameObject networkingMenu;
    GameObject gameMenu;
    GameObject startButton;
    GameObject hud;
    TMP_InputField nameField;
    TMP_InputField ipField;
    StageManager sm;
    //clientId, playername
    Dictionary<ulong, string> playerNames;
    Dictionary<ulong, int> woodTracker;
    Dictionary<ulong, int> pointTracker;
    [HideInInspector] public int treeCounter = 0;
    [HideInInspector] public bool roundActive = false;
    public GameObject statboxPrefab;
    List<Statbox> statboxes;
    GameObject exitButton;
    void Start()
    {
        sm = GameObject.Find("Stage").GetComponent<StageManager>();
        networkingMenu = transform.Find("Networking Menu").gameObject;
        gameMenu = transform.Find("Game Menu").gameObject;
        startButton = gameMenu.transform.Find("Go").gameObject;
        startButton.SetActive(false);
        hud = gameMenu.transform.Find("HUD").gameObject;
        exitButton = transform.Find("Exit").gameObject;
        exitButton.SetActive(false);

        gameMenu.SetActive(false);

        nameField = networkingMenu.transform.Find("Name").GetComponentInChildren<TMP_InputField>();
        ipField = networkingMenu.transform.Find("IP Address").GetComponentInChildren<TMP_InputField>();
        playerNames = new Dictionary<ulong, string>();
        woodTracker = new Dictionary<ulong, int>();
        pointTracker = new Dictionary<ulong, int>();
        statboxes = new List<Statbox>();
    }
    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            exitButton.SetActive(!exitButton.activeSelf);
        }
    }
    public void RestartScene() {
        if (IsServer) NetworkingManager.Singleton.StopHost();
        if (IsClient) NetworkingManager.Singleton.StopClient();
        SceneManager.LoadScene("SampleScene");
    }
    // called by server
    public void StartRound()
    {
        List<ulong> clientIds = new List<ulong>();
        foreach (KeyValuePair<ulong, int> entry in woodTracker)
        {
            clientIds.Add(entry.Key);
            // woodTracker[entry.Key] = 0;
        }
        InvokeClientRpcOnEveryone(CreateStatboxes, clientIds.ToArray());

        GameObject.Find("Stage").GetComponent<StageManager>().GenerateStage();
        roundActive = true;
        startButton.SetActive(false);        
    }
    [ClientRPC]
    void CreateStatboxes(ulong[] clientIds)
    {
        List<ulong> LclientIds = clientIds.ToList<ulong>();
        foreach (Statbox statbox in statboxes) {
            Destroy(statbox.gameObject);
        }
        statboxes = new List<Statbox>();
        foreach (ulong clientId in LclientIds)
        {
            GameObject IstatboxPrefab = Instantiate(statboxPrefab);
            IstatboxPrefab.transform.SetParent(hud.transform.Find("Stats").transform, false);
            statboxes.Add(IstatboxPrefab.GetComponent<Statbox>());
            IstatboxPrefab.GetComponent<Statbox>().clientId = clientId;
        }
        if (IsServer) UpdateStats();
    }
    [ClientRPC]
    public void UpdateStatsClient(string[] statTexts)
    {
        List<string> LstatTexts = statTexts.ToList<string>();
        for (int i = 0; i < LstatTexts.Count; i++)
        {
            ulong statboxId = statboxes[i].clientId;
            statboxes[i].UpdateTextWithText(LstatTexts[i]);
        }
    }
    void UpdateStats()
    {
        List<string> statTexts = new List<string>();
        foreach (KeyValuePair<ulong, string> entry in playerNames)
        {
            string statText = "";
            statText += entry.Value + "\n";
            statText += "<sprite=1>" + woodTracker[entry.Key] + "\n";
            statText += "<sprite=0>" + pointTracker[entry.Key];
            statTexts.Add(statText);
        }
        InvokeClientRpcOnEveryone(UpdateStatsClient, statTexts.ToArray());
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
        gameMenu.SetActive(true);

        InvokeServerRpc(ClientInitiatePlayer, clientId, name);
    }
    [ServerRPC(RequireOwnership = false)]
    void ClientInitiatePlayer(ulong clientId, string name)
    {
        playerNames.Add(clientId, name);
        woodTracker.Add(clientId, 0);
        pointTracker.Add(clientId, 0);
        Debug.Log(name + " (client " + clientId + ") has joined");
    }

    // Called from server
    public void AddWood(ulong clientId, int wood)
    {
        woodTracker[clientId] = woodTracker[clientId] + wood;
        if (woodTracker[clientId] < 0) {
            woodTracker[clientId] = 0;
        }
        // Debug.Log(GetName(clientId) + " has " + woodTracker[clientId] + " wood.");
        UpdateStats();
    }
    // Called from server
    public void AddPoint(ulong clientId)
    {
        string playerName = GetName(clientId);
        pointTracker[clientId] = pointTracker[clientId] + 1;
        // Debug.Log(GetName(clientId) + " has " + woodTracker[clientId] + " points.");
        UpdateStats();
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
    public void treeDestroyed()
    {
        treeCounter--;
        if (treeCounter <= 0)
        {
            ulong clientIdWinner = 0;
            int highestWood = 0;
            foreach (KeyValuePair<ulong, int> entry in woodTracker)
            {
                if (entry.Value > highestWood)
                {
                    highestWood = entry.Value;
                    clientIdWinner = entry.Key;
                }
            }
            AddPoint(clientIdWinner);
            Debug.Log(GetName(clientIdWinner) + " won with " + highestWood + ". They now have " + pointTracker[clientIdWinner] + " points.");
            roundActive = false;
            startButton.SetActive(true);
        }
    }
}
