using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICameraController
{
    ICamera Camera { get; }
}

public class CameraController : BasePlayerController, ICameraController, IReloadable
{
    [SerializeField] private BaseCamera _camera;

    [SerializeField] private float _cameraSpeed;
    [SerializeField] private Vector3 _cameraOffset;
    
    public ICamera Camera => _camera;

    private IPlayer _player = null;
    private ICameraInput _input = null;

    private float _xAxis = 0f;
    private float _yAxis = 0f;

    public override void Initialize(IPlayer player)
    {
        _player = player;

        _camera.Initialize();

        LocalInitialize();
    }

    [Client]
    private void LocalInitialize()
    {
        if (isLocalPlayer)
        { 
            _input = GetComponent<ICameraInput>();        
        }    
    }

    public void Reload() => RpcReload();

    [ClientRpc]
    private void RpcReload()
    {
        if (isLocalPlayer)
        {
            _camera.Reload();
        }
    } 

    [Server]
    public override void Enable() => RpcEnable();

    [ClientRpc]
    private void RpcEnable()
    {
        if (isLocalPlayer)
        {
            enabled = true;

            _camera.gameObject.SetActive(true);
        }
    }

    [Server]
    public override void Disable() => RpcDisable();

    [ClientRpc]
    private void RpcDisable()
    {
        if (isLocalPlayer)
        {
            enabled = false;
        }
    }


    [ClientCallback]
    private void LateUpdate() => ProcessLocalCameraInteraction();
    private void ProcessLocalCameraInteraction()
    {
        if (Application.isFocused == false) return;

        if (isLocalPlayer)
        { 
            Rotate();
            Move();            
        }
    }

    [Client]
    private void Rotate()
    {
        _xAxis += _input.MouseX * _cameraSpeed * Time.deltaTime;
        _yAxis -= _input.MouseY * _cameraSpeed * Time.deltaTime;

        _camera.SetRotation(Quaternion.Euler(_yAxis, _xAxis, 0f));
    }

    [Client]
    private void Move()
    {
        if (isLocalPlayer)
        {
            CmdSetCameraPosition(_cameraOffset + _player.Position);
            BaseSetCameraPosition(_cameraOffset + _player.Position);
        }
    }
    [Command]
    private void CmdSetCameraPosition(Vector3 position) => BaseSetCameraPosition(position);
    private void BaseSetCameraPosition(Vector3 position) => _camera.SetPosition(position);

    public override void Deactivate()
    {
        base.Deactivate();

        RpcDeactivate();
    }

    [ClientRpc]
    private void RpcDeactivate()
    {
        _camera.gameObject.SetActive(false);

        Clear();
    }

    protected override void Clear()
    {
        _camera = null;
        _player = null;
        _input = null;

        _xAxis = 0f;
        _yAxis = 0f;
    }
}
