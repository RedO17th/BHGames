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



public static class PlayerDataBus
{
    public static event Action<BaseContext> OnContextEvent;

    public static void SendContext(BaseContext context)
    {
        OnContextEvent?.Invoke(context);
    }
}

public class CollisionWithEnemy : BaseContext
{
    public IPlayer Player { get; private set; }
    public CollisionWithEnemy(IPlayer player)
    {
        Player = player;
    }
}

public class Dash : BaseContext
{
    public IPlayer Player { get; private set; }
    public bool Enabled { get; private set; }
    public Dash(IPlayer player, bool enabled) 
    {
        Player = player;
        Enabled = enabled;
    }
}