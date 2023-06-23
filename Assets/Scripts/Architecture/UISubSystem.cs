using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISubSystem : BaseNetworkSubSystem
{
    [SerializeField] private UILobbyController _lobbyController = null;

    private LobbyNetworkManager _lobbyManager = null;

    public override void Initialize(ILobbyNetManager lobbyManager)
    {
        _lobbyManager = lobbyManager as LobbyNetworkManager;
    }

    public override void SubScribe() 
    {
        SceneDataBus.OnContextEvent += ProcessContext;
    }

    private void ProcessContext(BaseContext cntxt)
    {
        if (cntxt is AddLobbyPlayer apContext)
        {
            TRpcEnableUIByClientRole(apContext.Player.Connection);
        }

        if (cntxt is ServerLoaded slContext)
        {
            EnableUIByServerRole();
        }
    }

    [TargetRpc]
    private void TRpcEnableUIByClientRole(NetworkConnectionToClient connection)
    {
        _lobbyController.ShowClientUI();
    }

    private void EnableUIByServerRole()
    {
        _lobbyController.ShowServerUI();
    }

    public override void StartSystem() 
    {
        //Где включать данный контроллер... При включении нужного состояния.
        _lobbyController.Enable();
    }

    public override void StopSystem()
    {
        _lobbyController.Disable();
    }

    public override void UnSubScribe()
    {
        SceneDataBus.OnContextEvent -= ProcessContext;
    }

    public override void Clear()
    {
        _lobbyManager = null;
    }
}
