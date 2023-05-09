using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : BasePlayerController
{
    [SerializeField] private float _movementSpeed = 0f;
    [SerializeField] private float _speedRotation = 0f;

    private IMovable _movable = null;

    private IPlayerKeyBoardInput _input = null;

    private Vector3 _direction = Vector3.zero;
    private Vector3 _directionRotation = Vector3.zero;

    public override void Initialize(IPlayer player)
    {
        _movable = player;

        _input = GetComponent<IPlayerKeyBoardInput>();
    }

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
}
