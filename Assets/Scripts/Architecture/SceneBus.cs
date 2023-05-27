using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SceneBus
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

public class CollisionWithEnemy : BaseContext
{
    public IPlayer Player { get; private set; }
    public CollisionWithEnemy(IPlayer player)
    {
        Player = player;
    }
}