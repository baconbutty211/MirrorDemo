using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

public class ClickManiaManager : NetworkBehaviour
{
    [SerializeField] GameObject[] grid;
    public static event System.Action<Team> OnTeamWins;
    private bool isGameOver = false;

    [Server]
    public override void OnStartServer()
    {
        Tile.OnClickServer += Tile_OnClick;
    }
    [Server]
    private void Tile_OnClick(Color teamColour)
    {
        CheckIsGoalMatched(teamColour);
    }
    [Server]
    private void CheckIsGoalMatched(Color teamColourThatJustClicked)
    {
        if(isGameOver)
        {
            Debug.LogWarning("Stop clicking tiles the ClickMania game is over");
            return;
        }

        bool isRedGoalMatch = IsGoalMatched(TeamGoals.instance.redTeamGoal, Team.Red.colour);
        bool isBlueGoalMatch = IsGoalMatched(TeamGoals.instance.blueTeamGoal, Team.Blue.colour);

        if (isRedGoalMatch)
        {
            if (Team.Red == teamColourThatJustClicked)
            {
                isGameOver = true;
                OnTeamWins?.Invoke(Team.Red);
            }
        }
        if (isBlueGoalMatch)
        {
            if (Team.Blue == teamColourThatJustClicked)
            {
                isGameOver = true;
                OnTeamWins?.Invoke(Team.Blue);
            }
        }
    }

    [Server]
    private bool IsGoalMatched(bool[] teamGoal, Color teamColour) 
    {
        if (grid.Length != teamGoal.Length)
            Debug.LogError($"Grid of tiles has different number of tiles to the teamGoal ({grid.Length} : {teamGoal.Length}, respectively)");

        for (int i = 0; i < grid.Length; i++)
            if (teamGoal[i])
                if (grid[i].GetComponent<SpriteRenderer>().color != teamColour)
                    return false;
        return true;
    }
    [Server]
    public override void OnStopServer()
    {
        Tile.OnClickServer -= Tile_OnClick;
    }
}