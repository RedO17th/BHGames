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

    public override void OnStartServer()
    {
        InitializeSubSystems();
        PrepareSubSystems();

        Debug.Log($"BaseSceneSystem.OnStartServer");
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

    //Проверить, где это нужно
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
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        var player = conn?.identity?.gameObject.GetComponent<IPlayer>();

        if (player != null)
        {
            Debug.Log($"SceneSystem.OnServerConnect");

            SceneDataBus.SendContext(new AddPlayer(player));
        }
    }
}
