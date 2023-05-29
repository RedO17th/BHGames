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

    public override void Enable()
    {
        base.Enable();

        PlayerDataBus.OnContextEvent += ProcessDashContext;
    }

    private void ProcessDashContext(BaseContext context)
    {
        if (context is DashContext dContext)
        {
            if (dContext.Player == _player)
            {
                ProcessDashAnimation(dContext.Enabled);
            }
        }
    }

    private void ProcessDashAnimation(bool enabled) => _animator.SetBool("Dash", enabled);

    private void Update() => ProcessAnimations();
    private void ProcessAnimations() => Move();
    private void Move() => _animator.SetFloat("Speed", _movementController.Speed);

    public override void Disable()
    {
        PlayerDataBus.OnContextEvent -= ProcessDashContext;

        base.Disable();
    }

    public override void Deactivate() => base.Deactivate();

    protected override void Clear()
    {
        _movementController = null;
        _player = null;
    }

}
