using System.Collections;
using UnityEngine;

public interface IDashMechanic : IEnabable, IDisabable 
{

}

public class DashMechanic : BaseMovementMechanic, IDashMechanic
{
    [Range(1f, 5f)]
    [SerializeField] private float _dashDistance = 3f;

    [Range(20f, 30f)]
    [SerializeField] private float _dashSpeed = 30f;

    private IMovementMechanic _movementMechanic = null;
    private IPlayerDashInput _input = null;
    private IMovable _movable = null;

    private bool _inProcess = false;

    private Coroutine _dashRoutine = null;

    private float _startedDashTime = 0f;
    [Space]
    public float _dashTime = 0f;

    public override void Initialize(IPlayerController controller)
    {
        var movementController = controller as IMovementController;
        
        _movable = movementController.Player;

        _input = GetComponent<IPlayerDashInput>();

        _movementMechanic = movementController.GetMechanic<IMovementMechanic>();
    }

    public void Enable()
    {
        enabled = true;
    }

    public void Disable()
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
            _inProcess = true;

            ProcessMechanic();
        }
    }

    private void ProcessMechanic() => _dashRoutine = StartCoroutine(Dash());
    private IEnumerator Dash()
    {
        PrepareToDash();

        while (CanContinue())
        {
            _movable.Dash(_movable.Forward * _dashSpeed * Time.deltaTime);

            yield return null;
        }

        CompleteDash();
    }

    private void PrepareToDash()
    {
        _movementMechanic.Disable();

        _startedDashTime = Time.time;

        _dashTime = _dashDistance / _dashSpeed;
    }

    private bool CanContinue() => Time.time < (_startedDashTime + _dashTime);

    private void CompleteDash()
    {
        _startedDashTime = 0f;
        _dashTime = 0f;

        _inProcess = false;

        _movementMechanic.Enable();
    }
}
