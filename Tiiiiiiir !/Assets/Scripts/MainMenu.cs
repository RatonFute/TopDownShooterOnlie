using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private NetworkManagerLobby _networkManager = null;

    [SerializeField] private GameObject _landingPagePanel = null;

    public void HostLobby()
    {
        _networkManager.StartHost();
        _landingPagePanel.SetActive(false);
    }
}
