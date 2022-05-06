using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

/// <summary>
/// TODO:
///     - (BUG) if client enters username before host, they're name is displayed twice
///     - Consider using TargetRpc to avoid removing all names and re-adding all names
///         - new player joins lobby
///         - ClientRpc the new players name
///         - TargetRpc the new player, all player names currently in the game
/// </summary>
public class PlayerLobbyTeamText : NetworkBehaviour
{
    [SerializeField]
    private Color myTeam;

    private void Start()
    {
        Player.OnPlayerJoinTeam += AddName;
        Player.OnPlayerLeaveTeam += RemoveName;

        InitializeNames();
    }

    [Command(requiresAuthority = false)]
    private void InitializeNames()
    {
        foreach (Player p in CustomNetworkManager.instance.playersList)
        {
            //In the case where the name already exists it is removed before it is added (to avoid duplication)
            RpcRemoveName(p.playerTeam, p.playerName);
            RpcAddName(p.playerTeam, p.playerName);
        }

        //foreach (Player p in CustomNetworkManager.instance.playersList)
        //{
        //    if (p.connectionToServer.identity == connectionToClient.identity)
        //        RpcAddName(p.playerTeam, p.playerName);
        //    else
        //        TargetAddName(connectionToClient, p.playerTeam, p.playerName);
        //}
    }
    

    private void AddName(Team team, string name)
    {
        if (isServer)
            RpcAddName(team, name);
        else
            CmdAddName(team, name);
    }
    private void RemoveName(Team team, string name) 
    {
        if (isServer)
            RpcRemoveName(team, name);
        else
            CmdRemoveName(team, name);
    }

    [Command]
    private void CmdAddName(Team team, string name) 
    {
        RpcAddName(team, name);
    }
    [Command]
    private void CmdRemoveName(Team team, string name) 
    {
        RpcRemoveName(team, name);
    }

    [ClientRpc]
    private void RpcAddName(Team team, string name)
    {
        //Debug.Log($"Adding {name} to team {team}");
        if (team != myTeam)
            return;
        if (string.IsNullOrEmpty(name))
            return;

        GetComponent<TextMeshProUGUI>().text += $"{name}\n";
    }
    [ClientRpc]
    private void RpcRemoveName(Team team, string name)
    {
        if (team != myTeam)
            return;
        if (string.IsNullOrEmpty(name))
            return;

        TextMeshProUGUI textMesh = GetComponent<TextMeshProUGUI>();
        string currTeamText = textMesh.text;
        textMesh.text = "";
        foreach (string str in currTeamText.Split('\n'))
            if (str != name && !string.IsNullOrWhiteSpace(str))
                AddName(team, str);
    }
    
    [TargetRpc]
    private void TargetAddName(NetworkConnection connection, Team team, string name) 
    {
        if (team != myTeam)
            return;
        if (string.IsNullOrEmpty(name))
            return;

        GetComponent<TextMeshProUGUI>().text += $"{name}\n";
    }
    

    private void OnDestroy()
    {
        Player.OnPlayerJoinTeam -= AddName;
        Player.OnPlayerLeaveTeam -= RemoveName;
    }
}