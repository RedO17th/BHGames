using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public interface IUINetworkPlayerInfo
{
    public NetworkIdentity Identity { get; }
    string Name { get; }
}

public class UINetworkPlayerInfo : IUINetworkPlayerInfo
{
    public NetworkIdentity Identity { get; private set; }
    public string Name { get; private set; }

    public UINetworkPlayerInfo(NetworkIdentity identity, string name)
    {
        Identity = identity;
        Name = name;
    }
}

public static class UINetworkPlayerInfoSerializer
{
    public static void WritePlayerInfo(this NetworkWriter writer, UINetworkPlayerInfo info)
    {
        writer.WriteNetworkIdentity(info.Identity);
        writer.WriteString(info.Name);
    }

    public static UINetworkPlayerInfo ReadPlayerInfo(this NetworkReader reader)
    {
        var identity = reader.ReadNetworkIdentity();
        var name = reader.ReadString();

        return new UINetworkPlayerInfo(identity, name);
    }
}
