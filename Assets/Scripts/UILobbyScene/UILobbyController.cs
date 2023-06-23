using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUILobbyController : IEnabable, IDisabable
{
    void ShowClientUI();
    void HideClientUI();

    void ShowServerUI();
    void HideServerUI();
}

public class UILobbyController : NetworkBehaviour, IUILobbyController
{
    [SerializeField] private UILobbyServerMenu _serverMenu;
    [SerializeField] private UILobbyClientMenu _clientMenu;

    [Server]
    public void Enable()
    {
        //RpcEnable();
        BaseEnable();
    }

    [ClientRpc]
    private void RpcEnable()
    {
        BaseEnable();
    }
    private void BaseEnable() => gameObject.SetActive(true);

    [Server]
    public void Disable()
    {
        //RpcDisable();
        BaseDisable();
    }
    
    [ClientRpc]
    private void RpcDisable() => BaseDisable();
    private void BaseDisable() => gameObject.SetActive(true);

    #region Client

    [Client]
    public void ShowClientUI()
    {
        _clientMenu.OnAddClientInfo += ProcessOnAddClientInfoEvent;
        _clientMenu.Enable();
    }

    [Client]
    public void HideClientUI()
    {
        _clientMenu.OnAddClientInfo -= ProcessOnAddClientInfoEvent;
        _clientMenu.Disable();
    }

    private void ProcessOnAddClientInfoEvent(string obj)
    {
        var info = new UINetworkPlayerInfo(netIdentity, obj);

        var writer = new NetworkWriter();
            writer.WritePlayerInfo(info);

        var data = writer.ToArray();

        CmdTransferPlayerInfo(data);
    }

    [Command(requiresAuthority = false)]
    private void CmdTransferPlayerInfo(byte[] info)
    {
        var reader = new NetworkReader(info);
        var playerInfo = reader.ReadPlayerInfo();

        Debug.Log($"UILobbyController.CmdTransferPlayerInfo");

        SceneDataBus.SendContext(new LobbyPlayerInfo(playerInfo));
    }

    #endregion

    #region Server

    public void ShowServerUI()
    {
        _serverMenu.Enable();
    }

    public void HideServerUI()
    {
        _serverMenu.Disable();
    }





    #endregion

}
