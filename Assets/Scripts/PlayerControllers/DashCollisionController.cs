using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public interface IDashCollisionController { }

public class DashCollisionController : BasePlayerController, IDashCollisionController
{
    private IPlayer _player = null;
    private IDashMechanic _dashMechanic = null;

    private Collider _trigger = null;

    public override void Initialize(IPlayer player)
    {
        _player = player;
        _dashMechanic = GetDashMechanic();

        _trigger = GetComponent<Collider>();
    }

    private IDashMechanic GetDashMechanic()
    { 
        return _player.GetController<IMovementController>().GetMechanic<IDashMechanic>();
    }

    public override void Enable()
    {
        base.Enable();

        _trigger.enabled = true;

        PlayerDataBus.OnContextEvent += ProcessContext;
    }

    private void ProcessContext(BaseContext context)
    {
        if (context is DamageContext dContext)
        {
            if (dContext.Player == _player)
            {
                ProcessDamageContext(dContext.Begin);
            }
        }
    }
    private void ProcessDamageContext(bool state) => _trigger.enabled = !state;

    #region Trigger
    private void OnTriggerEnter(Collider other)
    {
        var player = other?.attachedRigidbody?.GetComponent<IPlayer>();

        ProcessCollisionThrowDash(player);
    }
    private void ProcessCollisionThrowDash(IPlayer enemy)
    {
        if (enemy != null && _dashMechanic != null)
        {
            if (enemy.CanDamaged && _dashMechanic.InProcess)
            {
                enemy.Damage();

                ProcessCollisionWithEnemyContext();
            }
        }
    }
    private void ProcessCollisionWithEnemyContext() => PlayerDataBus.SendContext(new CollisionContext(_player));
    #endregion

    public override void Disable()
    {
        PlayerDataBus.OnContextEvent -= ProcessContext;

        _trigger.enabled = false;

        base.Disable(); 
    }

    protected override void Clear()
    {
        _dashMechanic = null;
        _player = null;

        _trigger = null;
    }
}
