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

    //TODO - ������������ � �������
    private List<IPlayer> _gamePlayers;
    public List<BaseNetworkLobbyPlayer> _lobbyPlayers;

    private ISceneHandler _sceneHandler = null;

    public override void OnStartServer()
    {
        _sceneHandler = new SceneHandler(this, _mapSet);

        _gamePlayers = new List<IPlayer>();
        _lobbyPlayers = new List<BaseNetworkLobbyPlayer>();
    }

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        if (_sceneHandler.CanAddNewPlayer())
        {
            var lobbyPlayer = Instantiate(_roomPlayerPrefab);
                lobbyPlayer.transform.parent = transform;

            _lobbyPlayers.Add(lobbyPlayer);

            NetworkServer.AddPlayerForConnection(conn, lobbyPlayer.gameObject);
        }
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

        foreach (var player in _lobbyPlayers)
        {
            if (player.IsReady == false)
                return false;
        }

        return true;
    }
    private void ProcessSceneLoadedEvent()
    {
        for (int i = 0; i < _lobbyPlayers.Count; i++)
        {
            var conn = _lobbyPlayers[i].connectionToClient;
            var player = Instantiate(_gamePlayerPrefab);
           
            if (player != null)
            {
                NetworkServer.RemovePlayerForConnection(conn, true);
                NetworkServer.AddPlayerForConnection(conn, player.gameObject);

                player.Initialize();
                player.Enable();

                _gamePlayers.Add(player.GetComponent<IPlayer>());

                SceneDataBus.SendContext(new NewClient(player));
            }
            else
            {
                Debug.LogError($"LobbyNetworkManager.ServerChangeScene: Player is null ");
            }
        }
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        TryRemovePlayer(conn);

        //[TODO] ���� _connections = 0, �� Lobby

        base.OnServerDisconnect(conn);
    }

    private void TryRemovePlayer(NetworkConnectionToClient conn)
    {
        if (conn.identity != null)
        {
            var player = conn.identity.gameObject.GetComponent<BaseNetworkPlayer>();

            if (player != null)
            {
                _gamePlayers.Remove(player);

                player.Disable();
                player.Deactivate();
                player.Remove();
            }
        }
    }

    public override void OnStopServer()
    {
        //_gamePlayers.Clear();

        _sceneHandler = null;
    }
}
