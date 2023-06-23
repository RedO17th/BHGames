using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISubSystem : BaseNetworkSubSystem
{
    [SerializeField] private UILobbyController _networkMenu = null;

    private LobbyNetworkManager _lobbyManager = null;

    public override void Initialize(ILobbyNetManager lobbyManager)
    {
        _lobbyManager = lobbyManager as LobbyNetworkManager;
    }

    public override void Prepare() 
    {
        SceneDataBus.OnContextEvent += ProcessContext;
    }

    private void ProcessContext(BaseContext cntxt)
    {
        if (cntxt is AddLobbyPlayer context)
        {
            TRpcEnableUIMenuByRole(context.Player.Connection);
        }
    }

    [TargetRpc]
    private void TRpcEnableUIMenuByRole(NetworkConnectionToClient connection)
    {
        //Где включать данный контроллер... При включении нужного состояния.
        _networkMenu.Enable();
        _networkMenu.StartClient();
    }

    public override void Stop() { }

    public override void Clear()
    {
        _lobbyManager = null;
    }
}
