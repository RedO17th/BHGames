using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovementMechanic : IEnabable, IDisabable { }

public class MovementMechanic : BaseMovementMechanic, IMovementMechanic
{
    [SerializeField] private float _movementSpeed = 0f;
    [SerializeField] private float _speedRotation = 0f;

    private ICamera _camera = null;
    private IPlayer _player = null;
    private IMovementInput _input = null;

    private Vector3 _direction = Vector3.zero;
    private Vector3 _directionRotation = Vector3.zero;

    public override void Initialize(IPlayerController controller)
    {
        var movementController = controller as IMovementController;

        _player = movementController.Player;

        _input = GetComponent<IMovementInput>();

        _camera = _player.GetController<ICameraController>().Camera;
    }

    private void Update() => ProcessInputAndMove();
    private void ProcessInputAndMove()
    {
        GetInputDirectionThrowCamera();
        Rotate();
        Move();
    }

    private void GetInputDirectionThrowCamera() => _direction = _camera.YRotation * _input.Direction;

    private void Rotate()
    {
        _directionRotation = new Vector3(_direction.x, 0f, _direction.z);
        if (_directionRotation.magnitude != 0f)
        {
            var rotation = Quaternion.LookRotation(_directionRotation);
            var targetRotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * _speedRotation);

            _player.Rotate(targetRotation);
        }
    }
    private void Move()
    {
        _player.Move(_direction * _movementSpeed * Time.deltaTime);
    }
}