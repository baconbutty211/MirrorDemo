using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

/// <summary>
/// Doesn't display on game load because it is a networked component
/// </summary>
public class Lobby : NetworkBehaviour
{
    [Header("Panels")]
    public GameObject usernamePanel;
    public GameObject lobbyPanel;

    private void Start()
    {
        PlayerUsername.OnSubmitUsername += OnSubmitUsername;

        usernamePanel.SetActive(true);
        lobbyPanel.SetActive(false);
    }

    private void OnSubmitUsername(string username)
    {
        usernamePanel.SetActive(false);
        lobbyPanel.SetActive(true);
    }

    private void OnDestroy()
    {
        PlayerUsername.OnSubmitUsername -= OnSubmitUsername;
    }
}
