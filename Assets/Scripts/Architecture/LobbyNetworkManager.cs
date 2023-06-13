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

    //public static event Action OnClientConnected;
    //public static event Action OnClientDisconnected;

    //Test
    public List<BaseNetworkLobbyPlayer> _roomPlayers = new List<BaseNetworkLobbyPlayer>();
    public List<BaseNetworkPlayer> _gamePlayers = new List<BaseNetworkPlayer>();

    private SceneHandler _sceneHandler = null;

    public override void OnStartServer()
    {
        _sceneHandler = new SceneHandler(this, _mapSet);
    }

    //public override void OnClientConnect()
    //{
    //    base.OnClientConnect();

    //    //OnClientConnected?.Invoke();
    //}
    //public override void OnClientDisconnect()
    //{
    //    base.OnClientDisconnect();

    //    //OnClientDisconnected?.Invoke();
    //}

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
    }
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        if (_sceneHandler.CanAddNewPlayer())
        {
            var roomPlayer = Instantiate(_roomPlayerPrefab);

            _roomPlayers.Add(roomPlayer);

            NetworkServer.AddPlayerForConnection(conn, roomPlayer.gameObject);
        }
    }

    //??
    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        //if (conn.identity != null)
        //{
        //    var player = conn.identity.GetComponent<NetworkRoomPlayerLobby>();

        //    RoomPlayers.Remove(player);

        //    NotifyPlayersOfReadyState();
        //}

        //base.OnServerDisconnect(conn);
    }
    public void NotifyPlayersOfReadyState()
    {
        //foreach (var player in RoomPlayers)
        //{
        //    player.HandleReadyToStart(IsReadyToStart());
        //}
    }
    //..

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

        foreach (var player in _roomPlayers)
        {
            if (player.IsReady == false)
                return false;
        }

        return true;
    }

    private void ProcessSceneLoadedEvent()
    {
        Debug.Log($"LobbyNetworkManager.ProcessSceneLoadedEvent");

        //for (int i = 0; i < _roomPlayers.Count; i++)
        //{
        //    var conn = _roomPlayers[i].connectionToClient;
        //    var player = Instantiate(_gamePlayerPrefab);

        //    if (player != null)
        //    {
        //        NetworkServer.Destroy(conn.identity.gameObject);
        //        NetworkServer.ReplacePlayerForConnection(conn, player.gameObject);

        //        //player.Initialize();
        //        //player.Enable();

        //        _gamePlayers.Add(player.GetComponent<BaseNetworkPlayer>());
        //    }
        //    else
        //    {
        //        Debug.Log($"LobbyNetworkManager.ServerChangeScene: Player is null ");
        //  
    }

    public override void OnStopServer()
    {
        _roomPlayers.Clear();
        
        _sceneHandler = null;
    }
}
