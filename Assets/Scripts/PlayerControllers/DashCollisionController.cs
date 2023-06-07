using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public interface IDashCollisionController { }

public class DashCollisionController : BasePlayerController, IDashCollisionController
{
    [SerializeField] private Collider _trigger = null;

    private IPlayer _player = null;
    private IDashMechanic _dashMechanic = null;

    public override void Initialize(IPlayer player)
    {
        _player = player;
        _dashMechanic = GetDashMechanic();
    }

    private IDashMechanic GetDashMechanic()
    { 
        return _player.GetController<IMovementController>().GetMechanic<IDashMechanic>();
    }

    [Server]
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

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        var enemy = other?.attachedRigidbody?.GetComponent<IPlayer>();

        if (enemy != null && enemy != _player)
        {
            ProcessCollisionThrowDash(enemy);
        }
    }
    private void ProcessCollisionThrowDash(IPlayer enemy)
    {
        if (enemy != null && _dashMechanic != null)
        {
            if (enemy.CanDamaged && _dashMechanic.InProcess)
            {
                enemy.Damage();

                //ProcessCollisionWithEnemyContext();
            }
        }
    }
    private void ProcessCollisionWithEnemyContext() => PlayerDataBus.SendContext(new CollisionContext(_player));
    #endregion

    [Server]
    public override void Disable()
    {
        PlayerDataBus.OnContextEvent -= ProcessContext;

        _trigger.enabled = false;

        base.Disable(); 
    }

    protected override void Clear()
    {
        RpcClear();
        BaseClear();
    }

    private void BaseClear()
    {
        _dashMechanic = null;
        _player = null;
    }

    [ClientRpc]
    private void RpcClear() => BaseClear();
}
