using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUINetworkMenu : IEnabable, IDisabable
{
    void StartClient();
    void StopClient();
}

public class UINetworkMenu : NetworkBehaviour, IUINetworkMenu
{
    [SerializeField] private UINetworkClientMenu _clientMenu;

    [SerializeField] private UINetworkPlayerInfo _playerInfoPrefab;

    public void Enable() => gameObject.SetActive(true);
    public void Disable() => gameObject.SetActive(false);

    public List<UINetworkPlayerInfo> _playerInfos = new List<UINetworkPlayerInfo>();

    [Client]
    public void StartClient()
    {
        Debug.Log($"UINetworkMenu.StartClient");

        _clientMenu.OnAddClientInfo += ProcessOnAddClientInfoEvent;
        _clientMenu.Enable();
    }

    [Client]
    public void StopClient()
    {
        _clientMenu.OnAddClientInfo -= ProcessOnAddClientInfoEvent;
        _clientMenu.Disable();
    }

    private void ProcessOnAddClientInfoEvent(string obj)
    {
        Debug.Log($"UINetworkMenu.ProcessOnAddClientInfoEvent: Name is {obj}");

        //Создавать на сервере
        var player = Instantiate(_playerInfoPrefab);
            player.name = obj;

        _playerInfos.Add(player);

        SendPlayerInfoToServer(player.Identity);
    }

    [Command(requiresAuthority = false)]
    private void SendPlayerInfoToServer(NetworkIdentity identity)
    {
        var player = identity.gameObject.GetComponent<IUINetworkPlayerInfo>();

        if (player != null)
        {
            Debug.Log($"Server: UINetworkMenu.SendPlayerInfoToServer: Name is { player.Name }");
        }
    }

    //Server
    public override void OnStartServer() { }
    public override void OnStopServer() { }


}
