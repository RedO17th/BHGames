using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISubSystem : BaseNetworkSubSystem
{
    private LobbyNetworkManager _lobbyManager = null;

    public override void Initialize(ILobbyNetManager lobbyManager)
    {
        _lobbyManager = lobbyManager as LobbyNetworkManager;
    }

    public override void Prepare() { }

    public override void Stop() { }

    public override void Clear()
    {
        _lobbyManager = null;
    }
}
