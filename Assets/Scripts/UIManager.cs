using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MLAPI;
using MLAPI.Transports.UNET;

public class UIManager : MonoBehaviour
{
    GameObject networkingMenu;
    GameObject gameMenu;
    GameObject startButton;
    TMP_InputField nameField;
    TMP_InputField ipField;
    StageManager sm;
    void Start()
    {
        sm = GameObject.Find("Stage").GetComponent<StageManager>();
        networkingMenu = transform.Find("Networking Menu").gameObject;
        gameMenu = transform.Find("Game Menu").gameObject;
        startButton = gameMenu.transform.Find("Go").gameObject;
        startButton.SetActive(false);
        nameField = networkingMenu.transform.Find("Name").GetComponentInChildren<TMP_InputField>();
        ipField = networkingMenu.transform.Find("IP Address").GetComponentInChildren<TMP_InputField>();
    }

    public void HostGame() {
        NetworkingManager.Singleton.StartHost();
        networkingMenu.SetActive(false);
        sm.initiate = true;
        startButton.SetActive(true);
    }
    public void JoinGame() {
        string ip = ipField.text;
        if (ip.Trim() == "") {
            NetworkingManager.Singleton.GetComponent<UnetTransport>().ConnectAddress = "127.0.0.1";
        }
        else {
            NetworkingManager.Singleton.GetComponent<UnetTransport>().ConnectAddress = ip;
        }
        NetworkingManager.Singleton.StartClient();
        
        networkingMenu.SetActive(false);
    }
}
