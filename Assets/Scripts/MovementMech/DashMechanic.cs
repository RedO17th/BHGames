using System.Collections;
using UnityEngine;

public interface IDashMechanic : IEnabable, IDisabable 
{
    bool InProcess { get; }
}

public class DashMechanic : BaseMovementMechanic, IDashMechanic
{
    [Range(1f, 5f)]
    [SerializeField] private float _dashDistance = 3f;

    [Range(20f, 30f)]
    [SerializeField] private float _dashSpeed = 30f;

    public bool InProcess => _inProcess;

    #region Internal variables

    private IMovementMechanic _movementMechanic = null;
    private IPlayerDashInput _input = null;
    private IPlayer _player = null;

    private Coroutine _dashRoutine = null;

    private bool _inProcess = false;

    private float _dashTime = 0f;
    private float _startedDashTime = 0f;

    #endregion

    public override void Initialize(IPlayerController controller)
    {
        var movementController = controller as IMovementController;
        
        _player = movementController.Player;

        _input = GetComponent<IPlayerDashInput>();

        _movementMechanic = movementController.GetMechanic<IMovementMechanic>();
    }

    public override void Disable()
    {
        StopMechanic();

        enabled = false;
    }
    private void StopMechanic()
    {
        if (_dashRoutine != null)
            StopCoroutine(_dashRoutine);

        CompleteDash();
    }

    private void Update() => ProcessInputAndDash();
    private void ProcessInputAndDash()
    {
        if (_input.Clicked && _inProcess == false)
        {
            ProcessMechanic();
        }
    }

    private void ProcessMechanic() => _dashRoutine = StartCoroutine(Dash());
    private IEnumerator Dash()
    {
        PrepareToDash();

        while (CanContinue())
        {
            _player.Dash(_player.Forward * _dashSpeed * Time.deltaTime);

            yield return null;
        }

        CompleteDash();
    }

    private void PrepareToDash()
    {
        _inProcess = true;

        _movementMechanic.Disable();

        _startedDashTime = Time.time;

        _dashTime = _dashDistance / _dashSpeed;

        PlayerDataBus.SendContext(new DashContext(_player, enabled: true));
    }

    private bool CanContinue() => Time.time < (_startedDashTime + _dashTime);

    private void CompleteDash()
    {
        _startedDashTime = 0f;
        _dashTime = 0f;

        _inProcess = false;

        _movementMechanic.Enable();

        PlayerDataBus.SendContext(new DashContext(_player, enabled: false));
    }

    public override void Deactivate()
    {
        base.Deactivate();
    }

    protected override void Clear()
    {
        _movementMechanic = null;
        _input = null;
        _player = null;

        _dashRoutine = null;
    }
}
