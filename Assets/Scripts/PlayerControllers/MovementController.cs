using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovementController
{
    IPlayer Player { get; }
    float Speed { get; }
    T GetMechanic<T>() where T : class;
}

public class MovementController : BasePlayerController, IMovementController
{
    [SerializeField] private BaseMovementMechanic[] _mechanics;

    public IPlayer Player { get; private set; } = null;
    public float Speed => _movementMechanic.NormalizeSpeed;

    private IMovementMechanic _movementMechanic = null;

    #region Systemic
    public virtual T GetMechanic<T>() where T : class
    {
        T result = null;

        foreach (var mechanic in _mechanics)
        {
            if (mechanic is T mech)
            {
                result = mech;
                break;
            }
        }

        return result;
    }
    #endregion

    public override void Initialize(IPlayer player)
    {
        Player = player;

        InitializeMechanics();

        _movementMechanic = GetMechanic<IMovementMechanic>();
    }
    private void InitializeMechanics()
    {
        foreach (var mechanic in _mechanics)
            mechanic.Initialize(this);
    }

    public override void Prepare() => PrepareMchanics();
    private void PrepareMchanics()
    {
        foreach (var mech in _mechanics)
        {
            mech.Prepare();
        }
    }

    [Server]
    public override void Enable()
    {
        base.Enable();

        EnableMechanics();
    }

    private void EnableMechanics()
    {
        foreach (var mech in _mechanics)
        {
            mech.Enable();
        }
    }

    [Server]
    public override void Disable()
    {
        DisableMechanics();

        base.Disable();
    }
    private void DisableMechanics()
    {
        foreach (var mech in _mechanics)
        {
            mech.Disable();
        }
    }

    public override void Deactivate()
    {
        DeactivateMechanics();

        base.Deactivate();

        RpcClear();
    }
    private void DeactivateMechanics()
    {
        foreach (var mech in _mechanics)
        {
            mech.Deactivate();
        }
    }

    [ClientRpc]
    private void RpcClear() => Clear();
    protected override void Clear()
    {
        _movementMechanic = null;
        Player = null;
    }
}
