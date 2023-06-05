using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICameraController
{
    ICamera Camera { get; }
}

public class CameraController : BasePlayerController, ICameraController
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

        RpcInitialize();
    }

    [Client]
    private void RpcInitialize()
    {
        if (isLocalPlayer)
        { 
            _input = GetComponent<ICameraInput>();        
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

    [Client]
    private void RpcDisable()
    {
        if (isLocalPlayer)
        {
            enabled = false;

            _camera.gameObject.SetActive(false);
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
            _camera.SetPosition(_cameraOffset + _player.Position);
        }
    }

    public override void Deactivate()
    {
        base.Deactivate();

        RpcClear();
    }

    [ClientRpc]
    private void RpcClear() => Clear();
    protected override void Clear()
    {
        _camera = null;
        _player = null;
        _input = null;

        _xAxis = 0f;
        _yAxis = 0f;
    }
}
