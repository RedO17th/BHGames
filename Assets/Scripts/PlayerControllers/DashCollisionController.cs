using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public interface IDashCollisionController : IReloadable
{
    int DashAmount { get; }
}

public class DashCollisionController : BasePlayerController, IDashCollisionController
{
    [SerializeField] private Collider _trigger = null;

    public int DashAmount => _dashAmount;

    private IPlayer _player = null;
    private IDashMechanic _dashMechanic = null;

    private int _dashAmount = 0;

    public override void Initialize(IPlayer player)
    {
        _player = player;
        _dashMechanic = GetDashMechanic();
    }
    private IDashMechanic GetDashMechanic()
    { 
        return _player.GetController<IMovementController>().GetMechanic<IDashMechanic>();
    }

    public void Reload() => ResetCounter();
    private void ResetCounter()
    {
        _dashAmount = 0;
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
                _dashAmount++;

                enemy.Damage();

                ProcessDashCollisionContext();
            }
        }
    }
    private void ProcessDashCollisionContext()
    {
        PlayerDataBus.SendContext(new DashCollision(_player, _dashAmount));
        SceneDataBus.SendContext(new DashAmount(_player.Name, _dashAmount));
    }
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
        _dashAmount = 0;

        _dashMechanic = null;
        _player = null;
    }

    [ClientRpc]
    private void RpcClear() => BaseClear();
}
