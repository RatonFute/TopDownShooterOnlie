using Mirror;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NetworkRoomPlayerLobby : NetworkBehaviour
{
    [SerializeField] private GameObject _lobbyUI = null;
    [SerializeField] private TMP_Text[] _playerNameTexts = new TMP_Text[10];
    [SerializeField] private TMP_Text[] _playerReadyTexts = new TMP_Text[10];
    [SerializeField] private Button _startGameButton = null;

    [SyncVar(hook = nameof(HandleDisplayNameChanged))]
    public string DisplayName = "Loading ...";
    [SyncVar(hook = nameof(HandleDisplayStatusChanged))]
    public bool IsReady = false;

    private bool isLeader;
    public bool IsLeader
    {
        set
        {
            isLeader = value;
            _startGameButton.gameObject.SetActive(value);
        }
    }

    private NetworkManagerLobby room;

    private NetworkManagerLobby Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as NetworkManagerLobby;
        }

    }

    public override void OnStartAuthority()
    {
        CmdSetDisplayName(PlayerNameInput.DisplayName);

        _lobbyUI.SetActive(true);
    }

    public override void OnStartClient()
    {
        Room.RoomPLayers.Add(this);
        UpdateDisplay();
    }


    public override void OnStopClient()
    {
        Room.RoomPLayers.Remove(this);

        UpdateDisplay();
    }

    public void HandleDisplayNameChanged(string oldValue, string newValue) => UpdateDisplay();
    public void HandleDisplayStatusChanged(bool oldValue, bool newValue) => UpdateDisplay();


    private void UpdateDisplay()
    {
        if(!authority)
        {
            foreach(var player in Room.RoomPLayers)
            {
                if(player.authority)
                {
                    player.UpdateDisplay();
                    break;
                }
            }
            return;
        }

        for (int i = 0;i < _playerNameTexts.Count();i++)
        {
            _playerNameTexts[i].text = "Waiting for player ...";
            _playerReadyTexts[i].text = string.Empty;
        }

        for (int i = 0; i < Room.RoomPLayers.Count(); i++)
        {
            _playerNameTexts[i].text = Room.RoomPLayers[i].DisplayName;
            _playerReadyTexts[i].text = Room.RoomPLayers[i].IsReady ? "<color=green>Ready</color>" : "<color=red>Not Ready</color>";
        }
    }

    public void HandleReadyToStart(bool readyToStart)
    {
        if (!isLeader) { return; }
        _startGameButton.interactable = readyToStart;
    }


    [Command]
    private void CmdSetDisplayName(string displayName)
    {
        DisplayName = displayName;
    }

    [Command]
    public void CmdReadyUp()
    {
        IsReady = !IsReady;
        Room.NotifyPlayersOfReadyState();
    }


    [Command]
    public void CmdStartGame()
    {
        if (Room.RoomPLayers[0].connectionToClient != connectionToClient) { return; }

        //start game

    }

}
