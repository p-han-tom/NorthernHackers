using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MLAPI;
public class Statbox : NetworkedBehaviour
{
    public ulong clientId;
    TextMeshProUGUI playerName;
    TextMeshProUGUI wood;
    TextMeshProUGUI points;
    void Start()
    {
        playerName = transform.Find("Name").GetComponent<TextMeshProUGUI>();
        wood = transform.Find("Wood").GetComponent<TextMeshProUGUI>();
        points = transform.Find("Points").GetComponent<TextMeshProUGUI>();
    }
    public void UpdateText(string name, int wood, int points) {
        Debug.Log(this.playerName.text+", "+this.wood.text+", "+this.points.text);
        this.playerName.text = name;
        this.wood.text = "<sprite=1>"+wood.ToString();
        this.points.text = "<sprite=0>"+points.ToString();
    }
}
