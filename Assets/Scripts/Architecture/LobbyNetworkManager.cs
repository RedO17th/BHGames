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
    private List<IPlayer> _gamePlayers;
    public List<BaseNetworkLobbyPlayer> _lobbyPlayers;

    private ISceneHandler _sceneHandler = null;

    public override void OnStartServer()
    {
        Debug.Log($"LobbyNetworkManager.OnStartServer");

        _sceneHandler = new SceneHandler(this, _mapSet);

        _gamePlayers = new List<IPlayer>();
        _lobbyPlayers = new List<BaseNetworkLobbyPlayer>();

        SceneDataBus.OnContextEvent += ProcessContextEvent;

        InitializeSubSystems();
    }

    private void InitializeSubSystems()
    {
        foreach (var system in _subSystems)
        {
            system.Initialize(this);
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

    public override void OnServerConnect(NetworkConnectionToClient conn) { }
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        if (_sceneHandler.CanAddNewPlayer())
        {
            var lobbyPlayer = Instantiate(_roomPlayerPrefab);
                lobbyPlayer.SetParent(transform);
                lobbyPlayer.SetName(Random.Range(0, 101).ToString());

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

        base.OnServerDisconnect(conn);

        //[TODO] Если _connections = 0, то Lobby
    }

    private void TryRemovePlayer(NetworkConnectionToClient conn)
    {
        if (conn.identity != null)
        {
            var player = conn.identity.gameObject.GetComponent<IPlayer>();

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

    public override void OnStopServer()
    {
        SceneDataBus.OnContextEvent -= ProcessContextEvent;

        //_gamePlayers.Clear();

        _sceneHandler = null;
    }
}
