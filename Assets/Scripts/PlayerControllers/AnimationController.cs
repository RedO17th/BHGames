using UnityEngine;
using Mirror;
using System;

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

    private void ProcessDashAnimation(bool enabled) => BaseDash(enabled);
    private void BaseDash(bool enabled) => _animator.SetBool("Dash", enabled);

    #region Move

    //[Client]
    private void Update() => ProcessAnimations();
    private void ProcessAnimations() => Move();
    private void Move()
    {
        Debug.Log($"AnimationController.Move: speed = {_movementController.Speed} ");

        BaseMove(_movementController.Speed);

        //CmdMove(_movementController.Speed);
    }

    private void BaseMove(float speed) => _animator.SetFloat("Speed", speed);

    //[Command(requiresAuthority = false)]
    //private void CmdMove(float speed)
    //{
    //    Debug.Log($"AnimationController.CmdMove: speed = { speed } ");

    //    //BaseMove(speed);
    //    _animator.SetFloat("Speed", speed);
    //}
    #endregion

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
