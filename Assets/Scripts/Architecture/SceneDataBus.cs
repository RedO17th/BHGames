using System;
using Mirror;

public static class SceneDataBus
{
    public static event Action<BaseContext> OnContextEvent;

    public static void SendContext(BaseContext context) 
    {
        OnContextEvent?.Invoke(context);
    }
}

public abstract class BaseContext { }
public class AddPlayer : BaseContext 
{
    public IPlayer Player { get; private set; }
    public AddPlayer(IPlayer player)
    {
        Player = player;
    }
}
public class DashAmount : BaseContext
{
    public string Winner { get; private set; }
    public int CollisionAmount { get; private set; }  
    public DashAmount(string name, int amount)
    {
        Winner = name;

        CollisionAmount = amount;
    }
}

#region Network
public class NewClient : BaseContext
{
    public IPlayer Client { get; private set; }
    public NewClient(IPlayer client)
    {
        Client = client;
    }
}

public class AddLobbyPlayer : BaseContext
{
    public ILobbyPlayer Player { get; private set; }

    public AddLobbyPlayer(ILobbyPlayer player)
    {
        Player = player;
    }
}

public class ServerLoaded : BaseContext
{
    public ServerLoaded() { }
}

public class LobbyPlayerInfo : BaseContext
{
    public IUINetworkPlayerInfo Info { get; private set; }

    public LobbyPlayerInfo(IUINetworkPlayerInfo info)
    {
        Info = info;
    }
}
#endregion

public static class PlayerDataBus
{
    public static event Action<BaseContext> OnContextEvent;

    public static void SendContext(BaseContext context)
    {
        OnContextEvent?.Invoke(context);
    }
}

//BasePlayerContext ???
public class DashCollision : BaseContext
{
    public IPlayer Player { get; private set; }
    public int Amount { get; private set; }
    public DashCollision(IPlayer player, int amount)
    {
        Player = player;
        Amount = amount;
    }
}

//BasePlayerContext ???
public class DashContext : BaseContext
{
    public IPlayer Player { get; private set; }
    public bool Enabled { get; private set; }
    public DashContext(IPlayer player, bool enabled) 
    {
        Player = player;
        Enabled = enabled;
    }
}

//BasePlayerContext ???
public class DamageContext : BaseContext
{
    public IPlayer Player { get; private set; }
    public bool Begin { get; private set; }

    public DamageContext(IPlayer player, bool begin)
    {
        Player = player;
        Begin = begin;
    }
}

