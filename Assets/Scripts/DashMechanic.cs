using System.Collections;
using UnityEngine;

public interface IDashMechanic : IEnabable, IDisabable 
{
    float Distance { get; }
}

public class DashMechanic : BaseMovementMechanic, IDashMechanic
{
    [SerializeField] private float _positionError = 0.05f;
    [SerializeField] private float _dashDistance = 3f;
    [SerializeField] private float _dashSpeed = 3f;

    public float Distance => _dashDistance;

    private IMovementMechanic _movementMechanic = null;
    private IPlayerDashInput _input = null;
    private IMovable _movable = null;

    private bool _inProcess = false;

    private Coroutine _dashRoutine = null;
    private Vector3 _targetPosition = Vector3.zero;

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

        _targetPosition = _movable.Position;

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

        while (DashNotCompleted())
        {
            _movable.Dash(Vector3.Lerp(_movable.Position, _targetPosition, _dashSpeed * Time.deltaTime));

            yield return null;
        }

        CompleteDash();
    }

    private void PrepareToDash()
    {
        _movementMechanic.Disable();

        _targetPosition = _movable.Position + _movable.Forward * _dashDistance;
    }

    private bool DashNotCompleted() => Vector3.Distance(_movable.Position, _targetPosition) > _positionError;

    private void CompleteDash()
    {
        _movable.Dash(_targetPosition);

        _targetPosition = Vector3.zero;

        _movementMechanic.Enable();

        _inProcess = false;
    }
}
