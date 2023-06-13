using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseNetworkLobbyPlayer : NetworkBehaviour
{
    public bool IsReady { get; private set; } = true;
}
