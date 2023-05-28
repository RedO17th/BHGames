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

    public override void Initialize(IPlayer player)
    {
        _player = player;
        _dashMechanic = GetDashMechanic();
    }

    private IDashMechanic GetDashMechanic()
    { 
        return _player.GetController<IMovementController>().GetMechanic<IDashMechanic>();
    }

    private void OnTriggerEnter(Collider other)
    {
        var player = other?.attachedRigidbody?.GetComponent<IPlayer>();

        ProcessCollisionThrowDash(player);
    }
    private void ProcessCollisionThrowDash(IPlayer player)
    {
        if (player != null && _dashMechanic.InProcess)
        {
            ProcessCollisionWithEnemyContext();
        }
    }

    private void ProcessCollisionWithEnemyContext() => PlayerDataBus.SendContext(new CollisionWithEnemy(_player));
}
