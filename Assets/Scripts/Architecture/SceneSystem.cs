using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISceneSystem { }

public abstract class BaseSceneSystem : MonoBehaviour, ISceneSystem
{
    [SerializeField] protected BaseSubSystem[] _subSystems;

    protected virtual void Awake() { }
    protected virtual void Start() 
    {
        InitializeSubSystems();
        PrepareSubSystems();
    }

    protected virtual void InitializeSubSystems()
    {
        foreach (var system in _subSystems)
            system.Initialize(this);
    }

    protected virtual void PrepareSubSystems()
    {
        foreach (var system in _subSystems)
            system.Prepare();
    }

    protected virtual void OnDisable()
    {
        foreach (var system in _subSystems)
            system.Clear();

    }
} 

public class SceneSystem : BaseSceneSystem
{


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneBus.SendContext(new CreatePlayer());
        }
    }
}
