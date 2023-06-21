using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISubSystem : BaseNetworkSubSystem
{
    [SerializeField] private UINetworkMenu _networkMenu = null;

    private LobbyNetworkManager _lobbyManager = null;

    public override void Initialize(ILobbyNetManager lobbyManager)
    {
        _lobbyManager = lobbyManager as LobbyNetworkManager;
    }

     

    //[TargetRpc]
    //private void SomeMeth(NetworkConnectionToClient target)
    //{ 
        
    //}

    public override void Prepare() 
    {
        SceneDataBus.OnContextEvent += ProcessContext;
    }

    [ClientCallback]
    private void OnEnable() => Debug.Log($"UISubSystem.LogMsg: OnEnable ");

    private void ProcessContext(BaseContext cntxt)
    {
        if (cntxt is AddLobbyPlayer context)
        {
            //_networkMenu.Enable();

            LogMsg("Context");
        }
    }

    [ClientRpc]
    private void LogMsg(string msg)
    {
        Debug.Log($"UISubSystem.LogMsg: {msg} ");
    }

    public override void Stop() { }

    public override void Clear()
    {
        _lobbyManager = null;
    }
}
