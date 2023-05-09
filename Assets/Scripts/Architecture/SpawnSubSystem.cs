using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSubSystem : BaseSubSystem
{
    [SerializeField] private BasePlayer _playerPrefab;

    [SerializeField] private BaseSpawnPoint[] _points;

    private SceneSystem _sceneSystem = null;

    public override void Initialize(ISceneSystem sceneSystem)
    {
        _sceneSystem = sceneSystem as SceneSystem;
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

        foreach (var point in _points)
        {
            if (point.IsBusy == false)
            {
                result = point;
                result.SetBusyState();

                break;
            }
        }

        return result;
    }

    public override void Clear()
    {
        SceneBus.OnContextEvent -= ProcessContextSignal;

        _sceneSystem = null;
    }
}
