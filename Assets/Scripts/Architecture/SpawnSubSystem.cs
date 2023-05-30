using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SpawnSubSystem : BaseSubSystem
{
    [SerializeField] private BasePlayer _playerPrefab;

    [SerializeField] private BaseSpawnPoint[] _points;

    private SceneSystem _sceneSystem = null;

    private List<ISpawnPoint> _freePoints;

    private List<IPlayer> _players;

    public override void Initialize(ISceneSystem sceneSystem)
    {
        _sceneSystem = sceneSystem as SceneSystem;

        InitializeFreePoints();
    }

    private void InitializeFreePoints()
    {
        _freePoints = new List<ISpawnPoint>();

        _players = new List<IPlayer>();

        foreach (var point in _points)
        {
            _freePoints.Add(point);
        }
    }

    public override void Prepare()
    {
        SceneDataBus.OnContextEvent += ProcessContextSignal;
    }

    //[Server]
    private void ProcessContextSignal(BaseContext context)
    {
        if (context is AddPlayer cContext)
        {
            ProcessAddPlayerContext(cContext.Player);
        }
    }

    private void ProcessAddPlayerContext(IPlayer player)
    {
        var point = GetSpawnPoint();

        if (point != null)
        {
                player.Initialize();    
                player.SetPosition(point.Position);

                player.Activate();

            _players.Add(player);
        }
    }

    private ISpawnPoint GetSpawnPoint()
    {
        ISpawnPoint result = null;

        if (_freePoints.Count > 0)
        {
            result = _freePoints[Random.Range(0, _freePoints.Count)];

            _freePoints.Remove(result);
        }

        return result;
    }

    public override void Stop()
    {
        foreach (var player in _players)
        {
            player.Deactivate();
        }

        _players.Clear();
    }

    public override void Clear()
    {
        SceneDataBus.OnContextEvent -= ProcessContextSignal;

        _sceneSystem = null;
    }
}
