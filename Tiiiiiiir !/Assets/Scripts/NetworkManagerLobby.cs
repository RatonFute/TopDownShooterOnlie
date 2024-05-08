using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;


public class NetworkManagerLobby : NetworkManager
{
    [SerializeField] private int _minPlayers = 2;
    [Scene][SerializeField] private string _menuScene = string.Empty;

    [SerializeField] private NetworkRoomPlayerLobby _roomPlayerPrefab = null;

    [SerializeField] private NetworkGamePlayerLobby _gamePlayerPrefab = null;

    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;
    public static event Action<NetworkConnection> OnServerReadied;

    public List<NetworkRoomPlayerLobby> RoomPLayers { get; } = new List<NetworkRoomPlayerLobby>();
    public List<NetworkGamePlayerLobby> GamePLayers { get; } = new List<NetworkGamePlayerLobby>();

    public override void OnStartServer()
    {
        spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();
    }

    public override void OnStartClient()
    {
        var spawnablePrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs");

        foreach (var prefab in spawnablePrefabs)
        {
            NetworkClient.RegisterPrefab(prefab);
        }
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();
        OnClientConnected?.Invoke();
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();
        OnClientDisconnected?.Invoke();
    }

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        if (numPlayers >= maxConnections)
        {
            conn.Disconnect();
            return;
        }

        if (SceneManager.GetActiveScene().path != _menuScene)
        {
            conn.Disconnect();
            return;
        }
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        if (SceneManager.GetActiveScene().path == _menuScene)
        {
            bool isLeader = RoomPLayers.Count == 0;

            NetworkRoomPlayerLobby roomPlayerInstance = Instantiate(_roomPlayerPrefab);

            roomPlayerInstance.IsLeader = isLeader;

            NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);

        }
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        if (conn.identity == null)
        {
            var player = conn.identity.GetComponent<NetworkRoomPlayerLobby>();

            RoomPLayers.Remove(player);
            NotifyPlayersOfReadyState();
        }

        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer()
    {
        RoomPLayers.Clear();
    }

    public void NotifyPlayersOfReadyState()
    {
        foreach (var player in RoomPLayers)
        {
            player.HandleReadyToStart(IsReadyToStart());
        }
    }

    private bool IsReadyToStart()
    {
        if (numPlayers < _minPlayers) { return false; }

        foreach (var player in RoomPLayers)
        {
            if (!player.IsReady) { return false; }
        }

        return true;
    }


    public void StartGame()
    {
        if (SceneManager.GetActiveScene().path == _menuScene)
        {
            if (!IsReadyToStart()) { return; }
            ServerChangeScene("Game");
        }
    }
    //<>


    public override void ServerChangeScene(string newSceneName)
    {
        if (SceneManager.GetActiveScene().path == _menuScene && newSceneName.StartsWith("Game"))
        {
            for (int i = RoomPLayers.Count - 1; i >= 0; i--)
            {
                var conn = RoomPLayers[i].connectionToClient;
                var gamePlayerInstance = Instantiate(_gamePlayerPrefab);
                gamePlayerInstance.SetDisplayName(RoomPLayers[i].name);

                NetworkServer.Destroy(conn.identity.gameObject);
                NetworkServer.ReplacePlayerForConnection(conn, gamePlayerInstance.gameObject);
            }
        }

        base.ServerChangeScene(newSceneName);
    }
    public override void OnServerReady(NetworkConnectionToClient conn)
    {
        base.OnServerReady(conn);

        OnServerReadied?.Invoke(conn);
    }

}



