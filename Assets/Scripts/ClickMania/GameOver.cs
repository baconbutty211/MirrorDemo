using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

public class GameOver : NetworkBehaviour
{
    [SerializeField] TextMeshProUGUI gameOverTextObj;
    [SerializeField] [Range(0,1)] float tintIntensity = .5f;

    [Client]
    public override void OnStartClient()
    {
        gameObject.SetActive(false);
    }

    [Server]
    public override void OnStartServer()
    {
        ClickManiaManager.OnTeamWins += OnTeamWins;
    }
    [ClientRpc]
    private void OnTeamWins(Team winTeam)
    {
        gameObject.SetActive(true);
        GetComponent<Image>().color = new Color(winTeam.colour.r, winTeam.colour.g, winTeam.colour.b, tintIntensity);
        gameOverTextObj.text = $"GameOver {winTeam} won ClickMania.";
        gameOverTextObj.color = winTeam.colour;
    }

    [Server]
    public override void OnStopServer()
    {
        ClickManiaManager.OnTeamWins += OnTeamWins;
    }
}
