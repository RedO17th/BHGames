using System.Collections;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public interface IDashMechanic : IEnabable, IDisabable 
{
    float Distance { get; }
}

public class DashMechanic : BaseMovementMechanic, IDashMechanic
{
    [SerializeField] private float _dashDistance = 3f;
    [SerializeField] private float _dashSpeed = 3f;

    public float dt = 0f;

    public float Distance => _dashDistance;

    private IMovementMechanic _movementMechanic = null;
    private IPlayerDashInput _input = null;
    private IMovable _movable = null;

    private bool _inProcess = false;

    private Coroutine _dashRoutine = null;
    //private Vector3 _targetPosition = Vector3.zero;

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

        //_targetPosition = _movable.Position;

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

    [ContextMenu("Dash")]
    private void Test() => ProcessMechanic();

    private void ProcessMechanic() => _dashRoutine = StartCoroutine(Dash());
    private IEnumerator Dash()
    {
        //PrepareToDash();

        _movementMechanic.Disable();

        float startTime = Time.time;
        dt = _dashDistance / _dashSpeed;

        while (Time.time < startTime + dt)
        {
            _movable.Dash(_movable.Forward * _dashSpeed * Time.deltaTime);

            yield return null;
        }

        //CompleteDash();

        _movementMechanic.Enable();

        _inProcess = false;
    }

    private void PrepareToDash()
    {
        //_targetPosition = _movable.Position + _movable.Forward * _dashDistance;
    }

    private void CompleteDash()
    {
        //_movable.Dash(_targetPosition);

        //_targetPosition = Vector3.zero;

        _movementMechanic.Enable();

        _inProcess = false;
    }
}
