using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUINetworkLobbyMenu : IEnabable, IDisabable
{
    void StartClient();
    void StopClient();
}

public class UINetworkLobbyMenu : NetworkBehaviour, IUINetworkLobbyMenu
{
    [SerializeField] private UINetworkClientMenu _clientMenu;

    [Server]
    public void Enable()
    {
        RpcEnable();
        BaseEnable();
    }

    [ClientRpc]
    private void RpcEnable() => BaseEnable();
    private void BaseEnable() => gameObject.SetActive(true);

    [Server]
    public void Disable()
    {
        RpcDisable();
        BaseDisable();
    }

    [ClientRpc]
    private void RpcDisable() => BaseDisable();
    private void BaseDisable() => gameObject.SetActive(true);


    public void StartClient() => RpcStartLocalClient();

    [ClientRpc]
    private void RpcStartLocalClient()
    {
        if(isLocalPlayer)
        { 
            _clientMenu.OnAddClientInfo += ProcessOnAddClientInfoEvent;
            _clientMenu.Enable();            
        }
    }

    public void StopClient() => RpcStopLocalClient();

    [ClientRpc]
    private void RpcStopLocalClient()
    {
        if (isLocalPlayer)
        { 
            _clientMenu.OnAddClientInfo -= ProcessOnAddClientInfoEvent;
            _clientMenu.Disable();            
        }
    }

    private void ProcessOnAddClientInfoEvent(string obj)
    {
        Debug.Log($"UINetworkMenu.ProcessOnAddClientInfoEvent: Name is {obj}");

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

    //Server
    public override void OnStartServer() { }
    public override void OnStopServer() { }


}
