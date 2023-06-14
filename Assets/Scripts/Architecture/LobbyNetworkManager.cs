using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public interface ILobbyNetManager
{
    void ServerChangeScene(string newSceneName);
}

public class LobbyNetworkManager : NetworkManager, ILobbyNetManager
{
    [Header("Map set")]
    [SerializeField] private MapSet _mapSet;

    [Header("Settings")]
    [SerializeField] private int _minPlayers = 2;

    [Header("Room player")]
    [SerializeField] private BaseNetworkLobbyPlayer _roomPlayerPrefab = null;

    [Header("Game player")]
    [SerializeField] private BaseNetworkPlayer _gamePlayerPrefab = null;

    //TODO - Позаботиться о списках
    private List<NetworkConnectionToClient> _connections = new List<NetworkConnectionToClient>();
    public List<BaseNetworkPlayer> _gamePlayers = new List<BaseNetworkPlayer>();

    private SceneHandler _sceneHandler = null;

    public override void OnStartServer()
    {
        _sceneHandler = new SceneHandler(this, _mapSet);
    }

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        if (numPlayers >= maxConnections)
        {
            conn.Disconnect();
            return;
        }

        if (_sceneHandler.CanAddNewConnection() == false)
        {
            conn.Disconnect();
            return;
        }

        _connections.Add(conn);
    }
    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
         _connections.Remove(conn);

        base.OnServerDisconnect(conn);
    }

    [ContextMenu("Start game")]
    private void StartGame()
    {
        if (_sceneHandler.CanStartGame() && IsReadyToStart())
        {
            if (_sceneHandler.SwitchTo(SceneType.Game))
            {
                _sceneHandler.OnSceneLoaded += ProcessSceneLoadedEvent;
            }
        }
    }

    private bool IsReadyToStart()
    {
        //if (numPlayers < _minPlayers) { return false; }

        //foreach (var player in _roomPlayers)
        //{
        //    if (player.IsReady == false)
        //        return false;
        //}

        return true;
    }

    private void ProcessSceneLoadedEvent()
    {
        Debug.Log($"LobbyNetworkManager.ProcessSceneLoadedEvent: Connections { _connections.Count }");

        for (int i = 0; i < _connections.Count; i++)
        {
            var conn = _connections[i];
            var player = Instantiate(_gamePlayerPrefab);

            if (player != null)
            {
                _gamePlayers.Add(player.GetComponent<BaseNetworkPlayer>());

                NetworkServer.AddPlayerForConnection(conn, player.gameObject);

                player.Initialize();
                player.Enable();

                SceneDataBus.SendContext(new NewClient(player));
            }
            else
            {
                Debug.Log($"LobbyNetworkManager.ServerChangeScene: Player is null ");
            }
        }
    }

    public override void OnStopServer()
    {
        //_roomPlayers.Clear();
        
        _sceneHandler = null;
    }
}
