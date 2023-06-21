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

    [Server]
    public void Enable()
    {
        RpcEnable();
        BaseEnable();
    }

    [ClientRpc]
    private void RpcEnable() => BaseEnable();
    private void BaseEnable() => gameObject.SetActive(true);

    public void Disable() => gameObject.SetActive(false);


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

        var info = new UINetworkPlayerInfo(obj);

        var writer = new NetworkWriter();
            writer.WritePlayerInfo(info);

        var data = writer.ToArray();

        CmdItem(data);
    }

    [Command(requiresAuthority = false)]
    private void CmdItem(byte[] info)
    {
        var reader = new NetworkReader(info);
        var item = reader.ReadPlayerInfo();

        Debug.Log($"UINetworkMenu.CmdItem");
    }

    //Server
    public override void OnStartServer() { }
    public override void OnStopServer() { }


}
