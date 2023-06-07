using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Mirror;
using System;

public class UICollisionCounter : NetworkBehaviour, IEnabable, IDisabable
{
    [SerializeField] private GameObject _canvas;
    [SerializeField] private TextMeshProUGUI _text;

    private ICamera _cameraTarget = null;

    public void Initialize(ICollisionCounterController counter)
    {
        _cameraTarget = GetCamera(counter);
    }
    private ICamera GetCamera(ICollisionCounterController counter)
    {
        return counter.Player.GetController<ICameraController>().Camera;
    }

    public void SetAmount(int amount)
    {
        RpcSetAmount(amount);
        BaseSetAmount(amount);
    }

    [ClientRpc]
    private void RpcSetAmount(int amount) => BaseSetAmount(amount);
    private void BaseSetAmount(int amount)
    {
        _text.text = amount.ToString();
    }

    public void Enable()
    {
        RpcEnable();
        BaseEnable();
    }

    [ClientRpc]
    private void RpcEnable()
    {
        BaseEnable();

        Debug.Log($"UICollisionCounter.RpcEnable");
    }
    private void BaseEnable() => _canvas.SetActive(true);

    public void Disable()
    {
        RpcDisable();
        BaseDisable();
    }

    [ClientRpc]
    private void RpcDisable() => BaseDisable();
    private void BaseDisable() => _canvas.SetActive(false);

    //[ServerCallback]
    //private void Update()
    //{
    //    if (_cameraTarget != null)
    //    {
    //        //RpcLookAtTarget(_cameraTarget.Position);
    //        LookAtTarget(_cameraTarget.Position);
    //    }
    //}

    //[ClientRpc]
    //private void RpcLookAtTarget(Vector3 position) => LookAtTarget(position);
    //private void LookAtTarget(Vector3 position) => transform.LookAt(position);

    public void Clear()
    {
        _cameraTarget = null;
    }
}
