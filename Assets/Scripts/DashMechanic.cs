using System.Collections;
using UnityEngine;

public class DashMechanic : BaseMovementMechanic
{
    [SerializeField] private float _positionError = 0.05f;
    [SerializeField] private float _dashDistance = 3f;
    [SerializeField] private float _dashSpeed = 3f;

    private IMovementController _movementController = null;
    private IMovementMechanic _movementMechanic = null;
    private IPlayerDashInput _input = null;
    private IMovable _movable = null;

    private bool _inProcess = false;

    private Coroutine _dashRoutine = null;
    private Vector3 _targetPosition = Vector3.zero;

    public override void Initialize(IPlayerController controller)
    {
        _movementController = controller as IMovementController;
        
        _movable = _movementController.Movable;

        _input = GetComponent<IPlayerDashInput>();

        _movementMechanic = _movementController.GetMechanic<IMovementMechanic>();
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
