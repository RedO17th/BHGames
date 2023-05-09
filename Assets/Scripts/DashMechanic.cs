using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows;
using static UnityEditor.Experimental.GraphView.GraphView;

public class DashMechanic : BaseMovementMechanic
{
    [SerializeField] private float _dashDistane = 3f;

    private IMovementController _movementController = null;
    private IPlayerDashInput _input = null;

    private IMovementMechanic _movementMechanic = null;

    private IMovable _movable = null;

    private bool _inProcess = false;

    private Coroutine _dashRoutine = null;

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

            Debug.Log($"DashMechanic.ProcessInputAndDash");

            ProcessMechanic();
        }
    }

    private void ProcessMechanic()
    {
        _dashRoutine = StartCoroutine(Dash());

        

        //_movable.Dash(Vector3.zero);
    }

    private IEnumerator Dash()
    {
        _movementMechanic.Disable();




        return null;
    }
}
