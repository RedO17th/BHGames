using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICollisionCounterController
{
    IPlayer Player { get; }
}

public class CollisionCounterController : BasePlayerController, ICollisionCounterController
{
    [SerializeField] private UICollisionCounter _uiCounter;

    public IPlayer Player { get; private set; }

    private int _amountCollisions = 0;

    public override void Initialize(IPlayer player)
    {
        Player = player;

        _uiCounter.Initialize(this);
        _uiCounter.SetAmount(0);
    }

    public override void Enable()
    {
        base.Enable();

        _uiCounter.Enable();

        SceneBus.OnContextEvent += ProcessContext;
    }

    private void ProcessContext(BaseContext context)
    {
        if(IsNecessaryContext(context))
        {
            DisplayCollisionAmount();
        }
    }
    private bool IsNecessaryContext(BaseContext context)
    {
        bool result = false;

        if (context is CollisionWithEnemy ceContext)
        {
            if (ceContext.Player == Player)
            {
                result = true;
            }
        }

        return result;
    }
    private void DisplayCollisionAmount()
    {
        _amountCollisions++;

        _uiCounter.SetAmount(_amountCollisions);
    }

    public override void Disable()
    {
        base.Disable();

        _uiCounter.Disable();

        SceneBus.OnContextEvent -= ProcessContext;
    }
}
