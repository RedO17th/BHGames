using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovementController
{
    IMovable Movable { get; }
    T GetMechanic<T>() where T : class;
}

public class MovementController : BasePlayerController, IMovementController
{
    [SerializeField] private BaseMovementMechanic[] _mechanics;

    public IMovable Movable { get; private set; } = null;

    public override void Initialize(IPlayer player)
    {
        Movable = player;

        DefineMechanics();
        InitializeMechanics();
    }

    private void DefineMechanics()
    {
        _mechanics = GetComponents<BaseMovementMechanic>();
    }
    private void InitializeMechanics()
    {
        foreach (var mechanic in _mechanics)
            mechanic.Initialize(this);
    }

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
}
