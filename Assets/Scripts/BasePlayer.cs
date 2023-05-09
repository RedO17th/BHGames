using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public interface IPlayer : IActivatable, IDeactivatable, IMovable
{
    void Initialize();
    T GetController<T>() where T : class;
}

public class BasePlayer : MonoBehaviour, IPlayer
{
    [SerializeField] private BasePlayerController[] _controllers;

    public Vector3 Forward => transform.forward;
    public Vector3 Position
    {
        get => transform.position;
        protected set => transform.position = value;
    }

    public virtual void Initialize() => InitializeControllers();
    private void InitializeControllers()
    {
        foreach (var controller in _controllers)
            controller.Initialize(this);
    }

    public virtual T GetController<T>() where T : class
    {
        T result = null;

        foreach (var controller in _controllers)
        {
            if (controller is T mech)
            {
                result = mech;
                break;
            }
        }

        return result;
    }

    public void SetPosition(Vector3 position) => Position = position;

    public virtual void Activate()
    { 
        
    }

    public virtual void Rotate(Quaternion rotation) => transform.rotation = rotation;
    public virtual void Move(Vector3 position) => Position += position;
    public virtual void Dash(Vector3 position) => Position = position;

    public virtual void Deactivate()
    {

    }
}
