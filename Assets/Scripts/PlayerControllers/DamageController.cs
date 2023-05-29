using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageController : IDamagable { } 

public class DamageController : BasePlayerController, IDamageController
{
    [Range(1f, 5f)]
    [SerializeField] private float _time;


    public bool CanDamaged => _canDamaged;


    private IPlayer _player = null;

    private Coroutine _damageRoutine = null;

    private bool _canDamaged = true;

    public override void Initialize(IPlayer player)
    {
        _player = player;
    }

    public override void Prepare() { }

    public override void Enable() => base.Enable();
    public override void Disable()
    {
        if (_damageRoutine != null)
            StopCoroutine(DamageCoroutine());

        _damageRoutine = null;

        base.Disable();
    }

    public void Damage() => ProcessDamage();
    private void ProcessDamage()
    {
        if (_canDamaged)
        {
            _canDamaged = false;

            _damageRoutine = StartCoroutine(DamageCoroutine());
        }
    }

    private IEnumerator DamageCoroutine()
    {
        ProcessBeginMechanic();

        yield return new WaitForSeconds(_time);

        ProcessEndMechanic();

        _canDamaged = true;
    }

    private void ProcessBeginMechanic() => PlayerDataBus.SendContext(new DamageContext(_player, begin: true));
    private void ProcessEndMechanic() => PlayerDataBus.SendContext(new DamageContext(_player, begin: false));

    public override void Deactivate()
    {
        _canDamaged = true;

        base.Deactivate();
    }
    protected override void Clear() 
    {
        _player = null;
    }
}
