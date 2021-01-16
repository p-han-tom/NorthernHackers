using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MLAPI;
using MLAPI.Transports.UNET;

public class UIManager : MonoBehaviour
{
    GameObject networkingMenu;
    TMP_InputField ipField;
    void Start()
    {
        networkingMenu = transform.Find("Networking Menu").gameObject;
        ipField = networkingMenu.transform.GetComponentInChildren<TMP_InputField>();
    }

    public void HostGame() {
        NetworkingManager.Singleton.StartHost();
        networkingMenu.SetActive(false);
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
