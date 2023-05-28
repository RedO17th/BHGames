using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : BasePlayerController
{
    [SerializeField] private Animator _animator;

    private IPlayer _player = null;
    private IMovementController _movementController = null;

    public override void Initialize(IPlayer player)
    {
        _player = player;
    }

    public override void Prepare()
    {
        _movementController = _player.GetController<IMovementController>();
    }

    private void Update() => ProcessAnimations();
    private void ProcessAnimations()
    {
        Move();
    }

    private void Move() => _animator.SetFloat("Speed", _movementController.Speed);
}
