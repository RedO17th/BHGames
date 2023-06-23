using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public interface ILobbyNetManager
{
    bool IsServer { get; }

    void ServerChangeScene(string newSceneName);
}

public class LobbyNetworkManager : NetworkManager, ILobbyNetManager
{
    [Header("Server settings")]
    [SerializeField] private bool _isServer = true;

    [Header("Network subsystems")]
    [SerializeField] private BaseNetworkSubSystem[] _subSystems;

    [Header("Map set")]
    [SerializeField] private MapSet _mapSet;

    [Header("Settings")]
    [SerializeField] private int _minPlayers = 2;
    [SerializeField] private int _amountToWin = 1;

    [Header("Room player")]
    [SerializeField] private BaseNetworkLobbyPlayer _roomPlayerPrefab = null;

    [Header("Game player")]
    [SerializeField] private BaseNetworkPlayer _gamePlayerPrefab = null;

    public bool IsServer => _isServer;

    //TODO - Позаботиться о списках
    public List<BaseNetworkPlayer> _gamePlayers;
    public List<BaseNetworkLobbyPlayer> _lobbyPlayers;

    private ISceneHandler _sceneHandler = null;

    public override void OnStartServer()
    {
        Debug.Log($"LobbyNetworkManager.OnStartServer");

        _sceneHandler = new SceneHandler(this, _mapSet);

        _gamePlayers = new List<BaseNetworkPlayer>();
        _lobbyPlayers = new List<BaseNetworkLobbyPlayer>();

        InitializeSubSystems();
        SubScribeSubSystems();
        StartSubSystems();

        SceneDataBus.OnContextEvent += ProcessContextEvent;

        //Отправить Event о том, что Сервер загрузился
        SceneDataBus.SendContext(new ServerLoaded());
    }

    private void InitializeSubSystems()
    {
        foreach (var system in _subSystems)
        {
            system.Initialize(this);
        }
    }
    private void SubScribeSubSystems()
    {
        foreach (var system in _subSystems)
        {
            system.SubScribe();
        }
    }
    private void StartSubSystems()
    {
        foreach (var system in _subSystems)
        {
            system.StartSystem();
        }
    }

    public override void OnStopServer()
    {
        SceneDataBus.OnContextEvent -= ProcessContextEvent;

        _sceneHandler = null;

        UnSubScribeSubSystems();
        StopSubSystems();
    }
    private void UnSubScribeSubSystems()
    {
        foreach (var system in _subSystems)
        {
            system.UnSubScribe();
        }
    }
    private void StopSubSystems()
    {
        foreach (var system in _subSystems)
        {
            system.StopSystem();
        }
    }


    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        Debug.Log($"LobbyNetworkManager.OnServerConnect");
    }
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        Debug.Log($"LobbyNetworkManager.OnServerAddPlayer");

        if (_sceneHandler.CanAddNewPlayer())
        {
            var lobbyPlayer = Instantiate(_roomPlayerPrefab);
                lobbyPlayer.SetParent(transform);
                //lobbyPlayer.SetName(Random.Range(0, 101).ToString());

            _lobbyPlayers.Add(lobbyPlayer);

            NetworkServer.AddPlayerForConnection(conn, lobbyPlayer.gameObject);

            SceneDataBus.SendContext(new AddLobbyPlayer(lobbyPlayer));
        }
    }

    private void ProcessContextEvent(BaseContext context)
    {
        if (context is DashAmount daContext)
        {
            if (daContext.CollisionAmount == _amountToWin)
            {
                //[TODO] Заглушку
                Debug.Log($"LobbyNetworkManager.ProcessContextEvent");

                StartCoroutine(Timer());
            }
        }

        if (context is LobbyPlayerInfo iContext)
        {
            Debug.Log($"LobbyNetworkManager.ProcessContextEvent: iContext");

            var lobbyPlayerInfo = iContext.Info;

            foreach (var player in _lobbyPlayers)
            {
                if (player.Connection == lobbyPlayerInfo.Identity.connectionToClient)
                {
                    Debug.Log($"LobbyNetworkManager.ProcessContextEvent: Player is {lobbyPlayerInfo.Name} ");
                }
                else
                {
                    Debug.Log($"LobbyNetworkManager.ProcessContextEvent: No match by Connection");
                }
            }
        }
    }

    private IEnumerator Timer()
    {
        yield return new WaitForSeconds(3f);

        //Disable players
        foreach (var player in _gamePlayers)
            player.Disable();

        //Set position
        foreach (var player in _gamePlayers)
        {
            player.SetPosition(Vector3.zero);
        }

        //Reload
        foreach (var player in _gamePlayers)
        {
            player.Reload();
        }

        //Enabling
        foreach (var player in _gamePlayers)
        {
            player.Enable();
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
            var lPlayer = _lobbyPlayers[i];

            var conn = lPlayer.Connection;
            var player = Instantiate(_gamePlayerPrefab);
           
            if (player != null)
            {
                NetworkServer.RemovePlayerForConnection(conn, true);
                NetworkServer.AddPlayerForConnection(conn, player.gameObject);

                player.SetName(lPlayer.Name);
                player.Initialize();
                player.Enable();

                _gamePlayers.Add(player.GetComponent<BaseNetworkPlayer>());

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

        base.OnServerDisconnect(conn);

        //[TODO] Если _connections = 0, то Lobby
    }

    private void TryRemovePlayer(NetworkConnectionToClient conn)
    {
        if (conn.identity != null)
        {
            var player = conn.identity.gameObject.GetComponent<BaseNetworkPlayer>();

            if (player != null)
            {
                Debug.Log($"LobbyNetworkManager.TryRemovePlayer: Player name is {player.Name} ");

                _gamePlayers.Remove(player);

                player.Disable();
                player.Deactivate();
                player.Remove();
            }
        }
    }
}
