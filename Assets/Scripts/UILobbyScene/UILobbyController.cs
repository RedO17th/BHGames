using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUILobbyController : IEnabable, IDisabable
{
    void StartClient();
    void StopClient();

    void StartServer();
    void StopServer();
}

public class UILobbyController : NetworkBehaviour, IUILobbyController
{
    [SerializeField] private UILobbyServerMenu _serverMenu;
    [SerializeField] private UILobbyClientMenu _clientMenu;

    [Client]
    public void Enable()
    {
        BaseEnable();
    }
    private void BaseEnable() => gameObject.SetActive(true);

    [Client]
    public void Disable()
    {
        BaseDisable();
    }
    private void BaseDisable() => gameObject.SetActive(true);

    #region Client

    [Client]
    public void StartClient()
    {
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
        var info = new UINetworkPlayerInfo(obj);

        var writer = new NetworkWriter();
            writer.WritePlayerInfo(info);

        var data = writer.ToArray();

        CmdTransferPlayerInfo(data);
    }

    [Command(requiresAuthority = false)]
    private void CmdTransferPlayerInfo(byte[] info)
    {
        var reader = new NetworkReader(info);
        var item = reader.ReadPlayerInfo();

        Debug.Log($"UINetworkMenu.CmdItem: Name is { item.Name } ");
    }

    #endregion

    #region Server

    public void StartServer()
    {
        
    }

    public void StopServer()
    {
        
    }





    #endregion

}
