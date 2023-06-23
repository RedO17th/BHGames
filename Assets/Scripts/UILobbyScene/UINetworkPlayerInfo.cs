using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using static UnityEditor.Progress;

public interface IUINetworkPlayerInfo
{
    string Name { get; }
}

public class UINetworkPlayerInfo : IUINetworkPlayerInfo
{
    public string Name { get; private set; }

    public UINetworkPlayerInfo(string name)
    {
        Name = name;
    }
}

public static class UINetworkPlayerInfoSerializer
{
    public static void WritePlayerInfo(this NetworkWriter writer, UINetworkPlayerInfo info)
    {
        //writer.WriteNetworkIdentity(info.Identity);
        writer.WriteString(info.Name);
    }

    public static UINetworkPlayerInfo ReadPlayerInfo(this NetworkReader reader)
    {
        //var identity = reader.ReadNetworkIdentity();
        var name = reader.ReadString();

        //return new UINetworkPlayerInfo(identity, name);
        return new UINetworkPlayerInfo(name);
    }
}
