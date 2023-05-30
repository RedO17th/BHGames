using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private IRunInput _runInput = null;
    private IWalktInput _walkInput = null;

    private Vector3 _direction = Vector3.zero;
    private Vector3 _directionRotation = Vector3.zero;

    private float _currentSpeed = 0f;
    private float _minSpeed = 0f;
    private float _maxSpeed = 0f;

    private bool _isDeactivated = false;

    public override void Initialize(IPlayerController controller)
    {
        var movementController = controller as IMovementController;

        _player = movementController.Player;

        _walkInput = GetComponent<IWalktInput>();
        _runInput = GetComponent<IRunInput>();

        _maxSpeed = _runSpeed;
    }

    public override void Prepare()
    {
        _camera = _player.GetController<ICameraController>().Camera;
    }

    public override void Enable()
    {
        if (_isDeactivated == false)
            base.Enable();
    }

    public override void Disable()
    {
        base.Disable();
    }

    [Client]
    private void Update() => ProcessInputAndMove();
    private void ProcessInputAndMove()
    {
        GetInputDirectionThrowCamera();
        Rotate();

        DefineMovementSpeed();

        Move();
    }

    private void GetInputDirectionThrowCamera()
    {
        if (_camera != null && _walkInput != null)
            _direction = _camera.YRotation * _walkInput.Direction;
        else
            _direction = Vector3.zero;
    }

    private void Rotate()
    {
        _directionRotation = new Vector3(_direction.x, 0f, _direction.z);
        if (_directionRotation.magnitude != 0f)
        {
            var rotation = Quaternion.LookRotation(_directionRotation);
            var targetRotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * _rotationSpeed);

            _player?.Rotate(targetRotation);
        }
    }

    private void DefineMovementSpeed()
    {
        if (_direction.normalized != Vector3.zero)
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

    private void Move()
    {
        _player?.Move(_direction.normalized * _currentSpeed * Time.deltaTime);
    }

    public override void Deactivate() 
    {
         _isDeactivated = true;

        base.Deactivate();
    }
        
        
    protected override void Clear()
    {
        _camera = null;
        _player = null;

        _runInput = null;
        _walkInput = null;

        _direction = Vector3.zero;
        _directionRotation = Vector3.zero;

        _currentSpeed = 0f;
        _minSpeed = 0f;
        _maxSpeed = 0f;
    }
}