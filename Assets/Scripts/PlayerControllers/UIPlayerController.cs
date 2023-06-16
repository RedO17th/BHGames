using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public interface IUIPlayerController : IReloadable { }

public class UIPlayerController : BasePlayerController, IUIPlayerController
{
    [SerializeField] private GameObject _canvas;
    [SerializeField] private UICollisionCounter _uiCounter;
    [SerializeField] private TextMeshProUGUI _nameField;

    private IDashCollisionController _dashController = null;

    private IPlayer _player = null;
    private ICamera _targetCamera = null;

    public override void Initialize(IPlayer player)
    {
        if (isServerOnly)
        {
            _player = player;

            _dashController = _player.GetController<IDashCollisionController>();

            GetPlayerCamera();

            RpcSetCollisionAmount(0);
            BaseSetCollisionAmount(0);

            RpcSetPlayerNameToUI(_player.Name);
            BaseSetPlayerNameToUI(_player.Name);
        }
    }

    private void GetPlayerCamera()
    {
        _targetCamera = _player.GetController<ICameraController>().Camera;
    }

    [ClientRpc]
    private void RpcSetCollisionAmount(int amount) => BaseSetCollisionAmount(amount);
    private void BaseSetCollisionAmount(int amount) => _uiCounter.SetAmount(amount);

    [ClientRpc]
    private void RpcSetPlayerNameToUI(string name) => BaseSetPlayerNameToUI(name);
    private void BaseSetPlayerNameToUI(string name)
    {
        _nameField.text = name;
    }

    public void Reload() => ResetCounter();
    private void ResetCounter()
    {
        RpcSetCollisionAmount(0);
        BaseSetCollisionAmount(0);

        EnableUI();
        RpcEnableUI();
    }

    [Server]
    public override void Enable()
    {
        base.Enable();

        EnableUI();
        RpcEnableUI();

        SceneDataBus.OnContextEvent += ProcessContext;
        PlayerDataBus.OnContextEvent += ProcessContext;
    }

    private void EnableUI() => _canvas.SetActive(true);

    [ClientRpc]
    private void RpcEnableUI() => EnableUI();

    private void ProcessContext(BaseContext context)
    {
        ProcessCollisionContext(context);
        ProcessContextForPreviousClients(context);
    }

    private void ProcessCollisionContext(BaseContext context)
    {
        if (context is DashCollision dcContext)
        {
            if (dcContext.Player == _player)
            {
                DisplayCollisionAmount(dcContext.Amount);
            }
        }
    }
    private void DisplayCollisionAmount(int amountCollisions)
    {
        RpcSetCollisionAmount(amountCollisions);
        BaseSetCollisionAmount(amountCollisions);
    }

    private void ProcessContextForPreviousClients(BaseContext context)
    {
        if (context is NewClient ncContext)
        {
            if (ncContext.Client != _player)
            {
                ShowUI(_dashController.DashAmount);
                RpcSetPlayerNameToUI(_player.Name);
            }
        }
    }
    private void ShowUI(int amountCollisions)
    {
        RpcSetCollisionAmount(amountCollisions);
        BaseSetCollisionAmount(amountCollisions);

        EnableUI();
        RpcEnableUI();
    }

    [ServerCallback]
    private void Update() => ProcessLookAtCameraMechanic();
    private void ProcessLookAtCameraMechanic()
    {
        if (_targetCamera != null)
        {
            RpcLookAtCamera(_targetCamera.Position);
        }
    }

    [ClientRpc]
    private void RpcLookAtCamera(Vector3 position) => _canvas.transform.LookAt(position);

    [Server]
    public override void Disable()
    {
        base.Disable();

        DisableUI();
        RpcDisableUI();

        SceneDataBus.OnContextEvent -= ProcessContext;
        PlayerDataBus.OnContextEvent -= ProcessContext;
    }
    private void DisableUI() => _canvas.SetActive(false);

    [ClientRpc]
    private void RpcDisableUI() => DisableUI();

    [Server]
    public override void Deactivate()
    {
        RpcSetCollisionAmount(0);
        BaseSetCollisionAmount(0);

        DisableUI();
        RpcDisableUI();

        base.Deactivate();
    }

    protected override void Clear()
    {
        _player = null;
        _targetCamera = null;
    }
}
