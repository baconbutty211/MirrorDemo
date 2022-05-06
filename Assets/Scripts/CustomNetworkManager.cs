using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

/*
	Documentation: https://mirror-networking.gitbook.io/docs/components/network-manager
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkManager.html
*/



public class CustomNetworkManager : NetworkManager
{
    public static event System.Action OnGameStart;

    [Scene]
    [SerializeField]
    private string GameScene = string.Empty;

    public static CustomNetworkManager instance { get { return (CustomNetworkManager)singleton; } }
    public List<Player> playersList = new List<Player>();

    public override void OnStartServer()
    {
        base.OnStartServer();
        Player.OnPlayerCreated += OnPlayerCreated;
        Player.OnPlayerDestroyed += OnPlayerDestroyed;
        Player.OnPlayerIsReady += CheckAllPlayersReady;
    }
    public override void OnStopServer()
    {
        Player.OnPlayerCreated += OnPlayerCreated;
        Player.OnPlayerDestroyed += OnPlayerDestroyed;
        Player.OnPlayerIsReady += CheckAllPlayersReady;
        base.OnStopServer();
    }

    private void OnPlayerCreated(Player player)
    {
        playersList.Add(player);
        Debug.Log($"{playersList.Count} total players");
    }
    private void OnPlayerDestroyed(Player player)
    {
        playersList.Remove(player);
        Debug.Log($"{playersList.Count} total players");
    }

    [Server]
    private void CheckAllPlayersReady()
    {
        foreach (Player p in playersList)
            if (!p.playerIsReady)
                return;
        StartGame();
    }
    [Server]
    private void StartGame()
    {
        OnGameStart?.Invoke();
        //Change scene for all players to game scene
        ServerChangeScene(GameScene);
    }
}