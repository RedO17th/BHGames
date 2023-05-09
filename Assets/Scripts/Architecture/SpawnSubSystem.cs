using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSubSystem : BaseSubSystem
{
    [SerializeField] private BasePlayer _playerPrefab;

    [SerializeField] private BaseSpawnPoint[] _points;

    private SceneSystem _sceneSystem = null;

    private List<ISpawnPoint> _freePoints;

    public override void Initialize(ISceneSystem sceneSystem)
    {
        _sceneSystem = sceneSystem as SceneSystem;

        InitializeFreePoints();
    }

    private void InitializeFreePoints()
    {
        _freePoints = new List<ISpawnPoint>();

        foreach (var point in _points)
        {
            _freePoints.Add(point);
        }
    }

    public override void Prepare()
    {
        SceneBus.OnContextEvent += ProcessContextSignal;
    }

    private void ProcessContextSignal(BaseContext context)
    {
        if (context is CreatePlayer cContext)
        {
            CreatePlayer();
        }
    }

    private void CreatePlayer()
    {
        var point = GetSpawnPoint();

        if (point != null)
        {
            var player = Instantiate(_playerPrefab);
                player.Initialize();    

                player.SetPosition(point.Position);
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

    public override void Clear()
    {
        SceneBus.OnContextEvent -= ProcessContextSignal;

        _sceneSystem = null;
    }
}
