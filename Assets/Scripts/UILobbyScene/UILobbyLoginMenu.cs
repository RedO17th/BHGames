using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UILobbyLoginMenu : MonoBehaviour
{
    [SerializeField] private LobbyNetworkManager _networkManager;

    [SerializeField] private BaseServerPanel _serverPanel;
    [SerializeField] private BaseClientPanel _clientPanel;

    private void OnEnable()
    {
        _serverPanel.OnServerRun += ProcessRunServerEvent;
        _serverPanel.Enable();
    }

    private void DefinePanel()
    {
        //_networkManager.IsServerEnabled и тд...

        //Нет, использовать иной подход. На основе компиляции
    }

    private void ProcessRunServerEvent(string adress)
    {
        _serverPanel.OnServerRun -= ProcessRunServerEvent;
        _serverPanel.Disable();

        _networkManager.networkAddress = adress;
        _networkManager.StartServer();
    }

    private void OnDisable()
    {
        _serverPanel.OnServerRun -= ProcessRunServerEvent;
        _serverPanel.Disable();
    }
}
