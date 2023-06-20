using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public interface IUINetworkPlayerInfo : INamable
{
    NetworkIdentity Identity { get; }
}

public class UINetworkPlayerInfo : NetworkBehaviour, IUINetworkPlayerInfo
{
    public NetworkIdentity Identity => netIdentity;
    public string Name { get; private set; }


    public void SetName(string name) => Name = name;
}
