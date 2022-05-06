using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class TeamGoals : NetworkBehaviour
{
    [SerializeField] Image[] goalTiles;
    [Space]
    [SerializeField] [Range(0, 1)] float goalTileDensity = .5f;
    public bool[] blueTeamGoal; // Server only
    public bool[] redTeamGoal; // Server only

    // (Server only) instance of TeamGoals. Should store the blue & red team goals
    public static TeamGoals instance;

    
    [Server]
    public override void OnStartServer()
    {
        base.OnStartServer();
        instance = this;

        //Set team goals
        blueTeamGoal = new bool[goalTiles.Length];
        for (int i = 0; i < blueTeamGoal.Length; i++)
            blueTeamGoal[i] = Random.Range(0f, 1f) < goalTileDensity;
        redTeamGoal = new bool[goalTiles.Length];
        for (int i = 0; i < redTeamGoal.Length; i++)
            redTeamGoal[i] = Random.Range(0f, 1f) < goalTileDensity;
    }

    [Client]
    public override void OnStartClient()
    {
        base.OnStartClient();
        //Debug.Log("GetTeamGoalsCmd()");
        GetTeamGoalCmd(Player.localPlayer.playerTeam, Player.localPlayer.connectionToClient);
    }

    [Command(requiresAuthority = false)]
    public void GetTeamGoalCmd(Team requestTeam, NetworkConnectionToClient sender)
    {
        bool[] goalTiles;
        if (requestTeam == Team.Red)
            goalTiles = redTeamGoal;
        else if (requestTeam == Team.Blue)
            goalTiles = blueTeamGoal;
        else
        { Debug.LogError("Player should not be on null team"); return; }
        //Debug.Log("SetGoalTilesTarget()");
        SetGoalTilesTarget(sender, goalTiles);
    }

    [TargetRpc]
    public void SetGoalTilesTarget(NetworkConnection target, bool[] goalTiles)
    {
        if (goalTiles.Length != this.goalTiles.Length)
            Debug.LogError($"Input goal tiles had different number of tiles to the UI ({goalTiles.Length} : {this.goalTiles.Length}, respectively)");
        for (int i = 0; i < goalTiles.Length; i++)
            if (goalTiles[i])
                FillTile(i);
            else
                ClearTile(i);
        //Debug.Log($"Client is connected {target != null}");
        //Debug.Log("All tiles set client-side");
    }
    private void FillTile(int pos)
    {
        goalTiles[pos].color = Player.localPlayer.playerTeam.colour;
    }
    private void ClearTile(int pos)
    {
        goalTiles[pos].color = Team.Default.colour;
    }
}
