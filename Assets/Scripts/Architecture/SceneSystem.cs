using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public interface ISceneSystem { }

public abstract class BaseSceneSystem : NetworkManager, ISceneSystem
{
    [SerializeField] protected BaseSubSystem[] _subSystems;

    public override void Awake() => base.Awake();
    public override void Start() 
    {
        base.Start();

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

    protected virtual void OnEnable()
    {
        SceneDataBus.OnContextEvent += ProcessContext;
    }

    private void ProcessContext(BaseContext context)
    {
        if (context is DashAmount dContext)
        {
            if (dContext.CollisionAmount == 3)
            {
                //StopSystems();

                //Заглушка
                //SceneManager.LoadScene(0);
            }
        }
    }

    private void StopSystems()
    {
        SceneDataBus.OnContextEvent -= ProcessContext;

        foreach (var system in _subSystems)
            system.Stop();

        foreach (var system in _subSystems)
            system.Clear();
    }

    protected virtual void OnDisable() => StopSystems();
} 

public class SceneSystem : BaseSceneSystem
{
    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        SceneDataBus.SendContext(new CreatePlayer());
    //    }
    //}
}
