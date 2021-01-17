using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MLAPI;
public class Statbox : NetworkedBehaviour
{
    public ulong clientId;
    TextMeshProUGUI statsText;
    void Start()
    {
        statsText = transform.Find("Text").GetComponent<TextMeshProUGUI>();
    }
    public void UpdateText(string name, int wood, int points)
    {
        statsText.text = name + "\n<sprite=1>" + wood + "\n<sprite=0>" + points;
    }
    public void UpdateTextWithText(string text)
    {
        if (statsText != null)
        statsText.text = text;
    }
    public string GetText() { return statsText.text; }
}
