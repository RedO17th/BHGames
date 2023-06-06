using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public interface IMovementMechanic : IEnabable, IDisabable 
{
    float NormalizeSpeed { get; }
}

public class MovementMechanic : BaseMovementMechanic, IMovementMechanic
{
    [SerializeField] private float _walkSpeed = 0f;
    [SerializeField] private float _runSpeed = 0f;
    [SerializeField] private float _rotationSpeed = 0f;

    public float NormalizeSpeed => (_currentSpeed - _minSpeed) / (_maxSpeed - _minSpeed);

    private ICamera _camera = null;
    private IPlayer _player = null;

    private Vector3 _directionRotation = Vector3.zero;

    private float _currentSpeed = 0f;
    private float _minSpeed = 0f;
    private float _maxSpeed = 0f;

    private IRunInput _runInput = null;
    private IWalktInput _walkInput = null;

    private Vector3 _clientDirection = Vector3.zero;

    public override void Initialize(IPlayerController controller)
    {
        var movementController = controller as IMovementController;

        _player = movementController.Player;
        _maxSpeed = _runSpeed;

        RpcInitializeInputs();
    }

    [Client]
    private void RpcInitializeInputs()
    {
        _walkInput = GetComponent<IWalktInput>();
        _runInput = GetComponent<IRunInput>();
    }

    public override void Prepare()
    {
        _camera = _player.GetController<ICameraController>().Camera;
    }

    public override void Enable()
    {
        base.Enable();

        RpcLocalEnable();
    }

    [ClientRpc]
    private void RpcLocalEnable()
    {
        if (isLocalPlayer)
        {
            enabled = true;
        }
    }

    public override void Disable()
    {
        base.Disable();

        RpcLocalDisable();
    }

    [ClientRpc]
    private void RpcLocalDisable()
    {
        if (isLocalPlayer)
        { 
            enabled = false;
        }
    }

    #region Movement

    [ClientCallback]
    private void Update() => ProcessLocalInputAndMove();
    private void ProcessLocalInputAndMove()
    {
        if (Application.isFocused == false) return;

        GetInputDirectionThrowCamera();

        DefineMovementSpeed();

        CmdRotate(_clientDirection);
        CmdMove(_clientDirection, _currentSpeed);
    }

    [Client]
    private void GetInputDirectionThrowCamera()
    {
        if (_camera != null && _walkInput != null)
            _clientDirection = _camera.YRotation * _walkInput.Direction;
        else
            _clientDirection = Vector3.zero;
    }

    [Client]
    private void DefineMovementSpeed()
    {
        if (_clientDirection.normalized != Vector3.zero)
        {
            _currentSpeed = _walkSpeed;

            if (_runInput.IsRunning)
            {
                _currentSpeed = _runSpeed;
            }
        }
        else
        {
            _currentSpeed = 0f;
        }
    }

    [Command]
    private void CmdRotate(Vector3 direction)
    {
        _directionRotation = new Vector3(direction.x, 0f, direction.z);
        if (_directionRotation.magnitude != 0f)
        {
            var rotation = Quaternion.LookRotation(_directionRotation);
            var targetRotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * _rotationSpeed);

            _player.Rotate(targetRotation);
        }
    }

    [Command]
    private void CmdMove(Vector3 direction, float speed)
    {
        _player.Move(direction.normalized * speed * Time.deltaTime);
    }
    #endregion

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

        _runInput = null;
        _walkInput = null;

        _clientDirection = Vector3.zero;
        _directionRotation = Vector3.zero;

        _currentSpeed = 0f;
        _minSpeed = 0f;
        _maxSpeed = 0f;
    }
}