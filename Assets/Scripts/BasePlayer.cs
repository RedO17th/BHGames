using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayer : IActivatable, IDeactivatable, IMovable
{
    void Initialize();
}

public class BasePlayer : MonoBehaviour, IPlayer
{
    [Header("Controllers")]
    [SerializeField] private BasePlayerController[] _controllers;

    public void SetPosition(Vector3 position) => transform.position = position;

    public virtual void Initialize()
    {
        InitializeControllers();
    }

    private void InitializeControllers()
    {
        foreach (var controller in _controllers)
            controller.Initialize(this);
    }

    public virtual void Activate()
    { 
        
    }

    public virtual void Rotate(Quaternion rotation) => transform.rotation = rotation;
    public virtual void Move(Vector3 position) => transform.position += position;

    public virtual void Deactivate()
    {

    }
}
