using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.SceneManagement;

public interface ISceneSystem { }

public abstract class BaseSceneSystem : NetworkManager, ISceneSystem
{
    [SerializeField] protected BaseSubSystem[] _subSystems;

    private List<IPlayer> _players = null;

    public override void OnStartServer()
    {
        _players = new List<IPlayer>();

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

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        var player = conn?.identity?.gameObject.GetComponent<IPlayer>();

        if (player != null)
        {
            player.Initialize();
            player.Enable();

            //player.SetPosition(point.Position);

            _players.Add(player);

            SceneDataBus.SendContext(new NewClient());
        }
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        var player = conn.identity.gameObject.GetComponent<IPlayer>();

        if (player != null)
        {
            _players.Remove(player);

            player.Disable();
            player.Deactivate();
            player.Remove();
        }

        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer()
    {
        //StopSystems();

        foreach (var player in _players)
        {
            player.Disable();
        }

        foreach (var player in _players)
        { 
            player.Deactivate();
        }

        foreach (var player in _players)
        {
            player.Remove();
        }

        _players.Clear();
        _players = null;
    }

    private void StopSystems()
    {
        foreach (var system in _subSystems)
            system.Stop();

        foreach (var system in _subSystems)
            system.Clear();
    }
} 

public class SceneSystem : BaseSceneSystem
{

}
