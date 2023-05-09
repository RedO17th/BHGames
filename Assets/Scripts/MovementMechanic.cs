using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovementMechanic : IEnabable, IDisabable { }

public class MovementMechanic : BaseMovementMechanic, IMovementMechanic
{
    [SerializeField] private float _movementSpeed = 0f;
    [SerializeField] private float _speedRotation = 0f;

    private IMovementController _movementController = null;

    private IMovable _movable = null;
    private IPlayerMovementInput _input = null;

    private Vector3 _direction = Vector3.zero;
    private Vector3 _directionRotation = Vector3.zero;

    public override void Initialize(IPlayerController controller)
    {
        _movementController = controller as IMovementController;

        _movable = _movementController.Movable;

        _input = GetComponent<IPlayerMovementInput>();
    }

    public void Enable() => enabled = true;

    private void Update() => ProcessInputAndMove();
    private void ProcessInputAndMove()
    {
        GetInputDirection();
        Rotate();
        Move();
    }

    private void GetInputDirection() => _direction = _input.Direction;
    private void Rotate()
    {
        _directionRotation = new Vector3(_direction.x, 0f, _direction.z);
        if (_directionRotation.magnitude != 0f)
        {
            var rotation = Quaternion.LookRotation(_directionRotation);
            var targetRotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * _speedRotation);

            _movable.Rotate(targetRotation);
        }
    }
    private void Move()
    {
        _movable.Move(_direction * _movementSpeed * Time.deltaTime);
    }

    public void Disable() => enabled = false;
}