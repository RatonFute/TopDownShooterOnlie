using Mirror;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NetworkGamePlayerLobby : NetworkBehaviour
{
   

    [SyncVar]
    private string _displayName = "Loading ...";

    private NetworkManagerLobby room;
    private NetworkManagerLobby Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as NetworkManagerLobby;
        }

    }

    public override void OnStartClient()
    {
        DontDestroyOnLoad(gameObject);
        Room.GamePLayers.Add(this);
    }


    public override void OnStopClient()
    {
        Room.GamePLayers.Remove(this);
    }

    [Server]
    public void SetDisplayName(string displayName)
    {
        _displayName = displayName;
    }



}
