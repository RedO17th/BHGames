using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SceneDataBus
{
    public static event Action<BaseContext> OnContextEvent;

    public static void SendContext(BaseContext context) 
    {
        OnContextEvent?.Invoke(context);
    }
}

public abstract class BaseContext { }
public class CreatePlayer : BaseContext 
{
    public CreatePlayer() { }
}
public class DashAmount : BaseContext
{
    public int CollisionAmount { get; private set; }  
    public DashAmount(int amount)
    {
        CollisionAmount = amount;
    }
}



public static class PlayerDataBus
{
    public static event Action<BaseContext> OnContextEvent;

    public static void SendContext(BaseContext context)
    {
        OnContextEvent?.Invoke(context);
    }
}

//BasePlayerContext ???
public class CollisionContext : BaseContext
{
    public IPlayer Player { get; private set; }
    public CollisionContext(IPlayer player)
    {
        Player = player;
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