using Mirror;
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
        if (isServerOnly)
        { 
            Player = player;

            _uiCounter.Initialize(this);
            _uiCounter.SetAmount(0);

            _amountCollisions = 0;
        }
    }

    [Server]
    public override void Enable()
    {
        base.Enable();

        _uiCounter.Enable();

        SceneDataBus.OnContextEvent += ProcessContext;
        PlayerDataBus.OnContextEvent += ProcessContext;
    }

    private void ProcessContext(BaseContext context)
    {
        if (IsCollisionContext(context))
        {
            DisplayCollisionAmount();
        }

        ProcessNewClientContext(context);
    }
    //..
    private bool IsCollisionContext(BaseContext context)
    {
        bool result = false;

        if (context is CollisionContext ceContext)
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

        //SceneDataBus.SendContext(new DashAmount(_amountCollisions));
    }
    //..

    private void ProcessNewClientContext(BaseContext context)
    {
        if (context is NewClient ncContext)
        {
            ShowCollisionCounter();
        }
    }
    private void ShowCollisionCounter() => _uiCounter.Enable();


    [Server]
    public override void Disable()
    {
        base.Disable();

        _uiCounter.Disable();

        SceneDataBus.OnContextEvent -= ProcessContext;
        PlayerDataBus.OnContextEvent -= ProcessContext;
    }

    [Server]
    public override void Deactivate()
    {
        _uiCounter.SetAmount(0);
        _uiCounter.Disable();
        _uiCounter.Clear();

        base.Deactivate();
    }

    protected override void Clear()
    {
        Player = null;

        _amountCollisions = 0;
    }
}
