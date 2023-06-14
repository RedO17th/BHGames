using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public interface ISceneHandler 
{

    public event Action OnSceneLoaded;

    //[TODO] Обернуть правилами?
    bool CanAddNewPlayer();
    bool CanAddNewConnection();
    bool CanStartGame();

    bool IsGamePlay();

    bool SwitchTo(SceneType type);
}

public class SceneHandler : ISceneHandler
{
    public event Action OnSceneLoaded;

    private ILobbyNetManager _lobbyNetManager = null;

    private IReadOnlyCollection<ProjectScene> _scenes = null;

    private SceneType _currentScene = SceneType.Lobby;

    public SceneHandler(ILobbyNetManager manager, MapSet set)
    {
        _lobbyNetManager = manager;

        _scenes = set.Scenes;
    }

    public bool CanAddNewPlayer() => _currentScene == SceneType.Lobby;
    public bool CanAddNewConnection() => _currentScene == SceneType.Lobby;
    public bool CanStartGame() => _currentScene == SceneType.Lobby;
    public bool IsGamePlay() => _currentScene == SceneType.Game;

    public bool SwitchTo(SceneType type) => ProcessSceneSwitching(type);
    private bool ProcessSceneSwitching(SceneType type)
    {
        var scene = SceneIsExist(type);

        if (scene != null)
        {
            _currentScene = scene.Type;

            _lobbyNetManager.ServerChangeScene(scene.Name);

            SceneManager.sceneLoaded += ProcessSceneLoadedEvent;

            return true;
        }

        return false;
    }

    private ProjectScene SceneIsExist(SceneType type)
    {
        foreach (var scene in _scenes)
        {
            if (scene.Type == type)
            {
                return scene;
            }
        }

        return null;
    }

    private void ProcessSceneLoadedEvent(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= ProcessSceneLoadedEvent;

        OnSceneLoaded?.Invoke();
    }


}
