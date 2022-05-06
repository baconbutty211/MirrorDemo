using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Tile : NetworkBehaviour
{
    public static event System.Action<Tile> OnClickClient;
    public static event System.Action<Color> OnClickServer;

    [SerializeField] bool isInteractable = false;
    [SerializeField] CentreCamera cam;

    [SyncVar(hook=nameof(TileColourChanged))]
    [SerializeField] Color tileColour;
    private void TileColourChanged(Color _, Color newColour) 
    {
        GetComponent<SpriteRenderer>().color = newColour;
    }

    [Server]
    public override void OnStartServer()
    {
        tileColour = Team.Default.colour;
    }
    [Server]
    public override void OnStopServer()
    {

    }

    public override void OnStartClient()
    {
        cam.OnZoomIn += SetInteractable;
        cam.OnZoomOut += SetNotInteractable;
    }
    public void SetInteractable()
    {
        isInteractable = true;
    }
    public void SetNotInteractable()
    {
        isInteractable = false;
    }
    public override void OnStopClient()
    {
        cam.OnZoomIn -= SetInteractable;
        cam.OnZoomOut -= SetNotInteractable;
    }

    [Client]
    private void OnMouseDown()
    {
        if (isInteractable)
        {
            SetTileColour(Player.localPlayer.playerTeam.colour);
            OnClickClient?.Invoke(this);
        }
    }
    [Command(requiresAuthority = false)]
    private void SetTileColour(Color playerColour) 
    {
        if (tileColour != Team.Default.colour)
            tileColour = Team.Default.colour;
        else
            tileColour = playerColour;
        OnClickServer?.Invoke(playerColour);
        //Debug.Log($"Setting tile colour to {tileColour}");
    }

    



}
