using UnityEngine;
using Mirror;
using System;

public class AnimationController : BasePlayerController
{
    [SerializeField] private Animator _animator;

    private IPlayer _player = null;
    private IMovementController _movementController = null;

    public override void Initialize(IPlayer player) => LocalInitialize(player);
    private void LocalInitialize(IPlayer player)
    {
        if (isLocalPlayer)
        {
            _player = player;
        }
    }

    public override void Prepare() => LocalPrepare();
    private void LocalPrepare()
    {
        if (isLocalPlayer)
        {
            _movementController = _player.GetController<IMovementController>();
        }
    }

    public override void Enable() => RpcLocalEnable();

    [ClientRpc]
    private void RpcLocalEnable()
    {
        if (isLocalPlayer)
        {
            enabled = true;

            PlayerDataBus.OnContextEvent += ProcessDashContext;
        }
    }

    #region Dash

    [Client]
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
    private void ProcessDashAnimation(bool enabled)
    {
        BaseDash(enabled);

        //CmdDash(enabled);
    }
    private void BaseDash(bool enabled) => _animator.SetBool("Dash", enabled);

    //[Command]
    //private void CmdDash(bool enabled) => BaseDash(enabled);
    #endregion

    #region Move
    [ClientCallback]
    private void Update() => ProcessAnimations();
    private void ProcessAnimations() => Move();
    private void Move() => _animator.SetFloat("Speed", _movementController.Speed);
    #endregion

    public override void Disable() => RpcLocalDisable();

    [ClientRpc]
    private void RpcLocalDisable()
    {
        if (isLocalPlayer)
        { 
            enabled = false;

            PlayerDataBus.OnContextEvent -= ProcessDashContext;            
        }
    }

    public override void Deactivate() => LocalDeactivate();
    private void LocalDeactivate()
    {
        if (isLocalPlayer)
        {
            base.Deactivate();
        }
    }

    [Client]
    protected override void Clear()
    {
        _movementController = null;
        _player = null;
    }

}
