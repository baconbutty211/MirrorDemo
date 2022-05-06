using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Events;

[System.Serializable]
public struct Team
{
    public static Team Default = new Team(Color.white);
    public static Team Blue = new Team(Color.blue);
    public static Team Red = new Team(Color.red);

    #region Operators
    public static bool operator ==(Team a, Team b) 
    {
        return a.colour == b.colour;
    }
    public static bool operator ==(Team a, Color b) 
    {
        return a.colour == b;
    }

    public static bool operator !=(Team a, Team b) 
    {
        return !(a.colour == b.colour);
    }
    public static bool operator !=(Team a, Color b) 
    {
        return !(a.colour == b);
    }
    #endregion

    public Color colour;
    private Team(Color team)
    {
        colour = team;
    }
    private static Team GetTeamFromColour(Color colour) 
    {
        if (Blue == colour)
            return Blue;
        else if (Red == colour)
            return Red;
        else if (Default == colour)
            return Default;
        else
            Debug.LogError($"Should not be getting team from colour {colour}. Returning default.");
        return Default;
    }

    public override string ToString()
    {
        if(this == Red)
            return $"Red team";
        else if(this == Blue)
            return $"Blue team";
        else if (this == Default)
            return $"Default team";
        else
            return $"No team found. Colour:{colour}";
    }
}

[System.Serializable]
public class Player : NetworkBehaviour
{
    public static event System.Action<Player> OnPlayerCreated;
    public static event System.Action<Player> OnPlayerDestroyed;
    public static event System.Action<Team, string> OnPlayerJoinTeam;
    public static event System.Action<Team, string> OnPlayerLeaveTeam;
    public static event System.Action<string> OnPlayerNameChanged;
    public static event System.Action OnPlayerIsReady;

    public static Player localPlayer;

    private void Start()
    {
        if (isLocalPlayer)
            localPlayer = this;

        PlayerUsername.OnSubmitUsername += OnSubmitUsername;
        PlayerLobby.OnLobbyUiTeamChanged += OnLobbyUiTeamChanged;
        PlayerLobby.OnLobbyUiIsReadyChanged += OnLobbyUiIsReadyChanged;

        OnPlayerCreated?.Invoke(this);

        DontDestroyOnLoad(gameObject);
    }
    private void Update() { }

    private void OnDestroy()
    {
        OnPlayerDestroyed?.Invoke(this);

        OnPlayerCreated = null;
        OnPlayerDestroyed = null;
        OnPlayerJoinTeam = null;
        OnPlayerLeaveTeam = null;
    }

    #region SyncVars

    [Header("SyncVars")]

    [SerializeField]
    [SyncVar(hook = nameof(PlayerTeamChanged))] 
    public Team playerTeam = Team.Default;
    //Hook
    private void PlayerTeamChanged(Team currTeam, Team newTeam)
    {
        if (isServer)
        {
            OnPlayerLeaveTeam?.Invoke(currTeam, playerName);
            OnPlayerJoinTeam?.Invoke(newTeam, playerName);
        }
    }

    [SyncVar(hook = nameof(PlayerNameChanged))]
    public string playerName = "";
    //Hook
    private void PlayerNameChanged(string _, string newName)
    {
        OnPlayerNameChanged?.Invoke(newName);
    }

    [SyncVar(hook = nameof(PlayerIsLobbyChanged))]
    public bool playerIsLobby = false;
    //Hook
    private void PlayerIsLobbyChanged(bool _, bool newIsLobby) 
    {
        
    }

    [SyncVar(hook = nameof(PlayerIsReadyChanged))]
    public bool playerIsReady = false;
    //Hook
    private void PlayerIsReadyChanged(bool _, bool isReady)
    {
        if (isReady)
            OnPlayerIsReady?.Invoke();
    }

    #endregion

    #region Commands
    [Command]
    public void SetPlayerName(string newName)
    {
        playerName = newName;
    }
    [Command]
    public void SetPlayerTeam(Team newTeam)
    {
        //Debug.Log($"Setting {playerName} to {newTeam}");
        playerTeam = newTeam;
    }
    [Command]
    public void SetPlayerIsLobby(bool isLobby)
    {
        playerIsLobby = isLobby;
    }
    [Command]
    public void SetPlayerIsReady(bool isReady)
    {
        playerIsReady = isReady;
    }

    #endregion

    private void OnSubmitUsername(string username)
    {
        if (hasAuthority)
        {
            SetPlayerName(username);
            SetPlayerIsLobby(true);
        }
    }

    private void OnLobbyUiIsReadyChanged(bool isReady)
    {
        if (hasAuthority)
            SetPlayerIsReady(isReady);
    }
    private void OnLobbyUiTeamChanged(Team team)
    {
        if (hasAuthority)
            SetPlayerTeam(team);
    }
}
