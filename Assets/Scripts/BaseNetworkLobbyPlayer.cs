using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILobbyPlayer : INamable
{
    NetworkConnectionToClient Connection { get; }
    bool IsReady { get; }

    void SetParent(Transform parent);
}
public class BaseNetworkLobbyPlayer : NetworkBehaviour, ILobbyPlayer
{
    public NetworkConnectionToClient Connection => connectionToClient;
    public bool IsReady { get; private set; } = true;
    public string Name { get; private set; } = string.Empty;

    public void SetName(string name)
    {
        Name = name;
    }

    public void SetParent(Transform parent)
    {
        transform.parent = parent;
    }
}
