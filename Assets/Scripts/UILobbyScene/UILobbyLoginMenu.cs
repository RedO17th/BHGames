using Mirror;
using System;
using UnityEngine;

public class UILobbyLoginMenu : MonoBehaviour
{
    [SerializeField] private LobbyNetworkManager _networkManager;

    //�������� � �������
    [SerializeField] private BaseServerMenu _serverPanel;
    [SerializeField] private BaseClientMenu _clientPanel;

    private void OnEnable() => DefinePanelByRole();
    private void DefinePanelByRole()
    {
        Action definer = _networkManager.IsServer ? DefineLikeServer : DefineLikeClient;
        definer.Invoke();
    }

    private void DefineLikeServer()
    {
        Debug.Log($"UILobbyLoginMenu.DefinePanel: Like server");

        _serverPanel.OnStartBynetAdress += ProcessRunServerEvent;
        _serverPanel.Enable();
    }

    private void ProcessRunServerEvent(string adress)
    {
        _serverPanel.OnStartBynetAdress -= ProcessRunServerEvent;
        _serverPanel.Disable();

        _networkManager.networkAddress = adress;
        _networkManager.StartServer();
    }


    private void DefineLikeClient()
    {
        Debug.Log($"UILobbyLoginMenu.DefinePanel: Like client");

        _clientPanel.OnStartBynetAdress += ProcessRunClientEvent;
        _clientPanel.Enable();
    }

    private void ProcessRunClientEvent(string adress)
    {
        _clientPanel.OnStartBynetAdress -= ProcessRunClientEvent;
        _clientPanel.Disable();

        _networkManager.networkAddress = adress;
        _networkManager.StartClient();
    }

    //��� �������� ��� ������� �� ������... ��� �������� ������ ������ ����������� ��� ��� ���������� "������� � ��������"
    //������� ������� ������� � UI ��� �������������...
}
