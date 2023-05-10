using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDashController : IEnabable, IDisabable { }

public class DashController : BasePlayerController, IDashController
{
    [SerializeField] private LayerMask _raysMask;

    [SerializeField] private Transform _shortRay;
    [SerializeField] private Transform _longRay;

    [SerializeField] private float _shortRayDistance;

    private IDashMechanic _dashMechanic = null;

    private bool _mechIsEnabled = true;

    private Ray _ray;

    private float _longRayDistance = 0f;

    public override void Initialize(IPlayer player)
    {
        _dashMechanic = player.GetController<IMovementController>().GetMechanic<IDashMechanic>();

        _longRayDistance = _dashMechanic.Distance;
    }

    public void Enable() => enabled = true;
    public void Disable() => enabled = false;

    private void Update() 
    {
        if (_mechIsEnabled)
        { 
            _ray = new Ray(_shortRay.position, _shortRay.forward);

            if (IsClose())
            {
                _mechIsEnabled = false;

                _dashMechanic?.Disable();
            }            
        }

        if (_mechIsEnabled == false)
        {
            _ray = new Ray(_longRay.position, _longRay.forward);

            if (IsFarEnough())
            {
                _mechIsEnabled = true;

                _dashMechanic?.Enable();
            }
        }
    }

    private bool IsClose() => Physics.Raycast(_ray, _shortRayDistance, _raysMask);
    private bool IsFarEnough() => Physics.Raycast(_ray, _longRayDistance, _raysMask) == false;
}
