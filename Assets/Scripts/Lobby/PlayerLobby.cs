using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;


/// <summary>
/// TODO:
///     - Create a ready button for the player, and add a field in the player class for ready (probably syncvar)
///     - Sometimes the team text fields are out of sync on the client, when the client joins the lobby
///         
/// </summary>
public class PlayerLobby : MonoBehaviour
{
    public static event System.Action<Team> OnLobbyUiTeamChanged;
    public static event System.Action<bool> OnLobbyUiIsReadyChanged;

    [Header("Join Team Buttons")]
    public Button JoinRedTeamBtn;
    public Button JoinBlueTeamBtn;
    [Header("Ready Button")]
    public Button ConfirmTeamButton;

    public void SetPlayerToRedTeam() 
    {
        JoinRedTeamBtn.interactable = false;
        JoinBlueTeamBtn.interactable = true;

        ConfirmTeamButton.interactable = true;

        OnLobbyUiTeamChanged?.Invoke(Team.Red);
        OnLobbyUiIsReadyChanged?.Invoke(false);
    }
    public void SetPlayerToBlueTeam() 
    {
        JoinRedTeamBtn.interactable = true;
        JoinBlueTeamBtn.interactable = false;

        ConfirmTeamButton.interactable = true;
        
        OnLobbyUiTeamChanged?.Invoke(Team.Blue);
        OnLobbyUiIsReadyChanged?.Invoke(false);
    }

    public void SetPlayerToReady() 
    {
        //JoinBlueTeamBtn.interactable = false;
        //JoinRedTeamBtn.interactable = false;

        ConfirmTeamButton.interactable = false;

        OnLobbyUiIsReadyChanged?.Invoke(true);
    }
}
