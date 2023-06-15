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

    private IPlayer _player = null;

    private int _amountCollisions = 0;

    public override void Initialize(IPlayer player)
    {
        if (isServerOnly)
        {
            _player = player;

            RpcSetCollisionAmount(0);
            BaseSetCollisionAmount(0);

            _amountCollisions = 0;

            RpcSetPlayerNameToUI(_player.Name);
            BaseSetPlayerNameToUI(_player.Name);
        }
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
        _amountCollisions = 0;

        RpcSetCollisionAmount(_amountCollisions);
        BaseSetCollisionAmount(_amountCollisions);

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
        if (context is CollisionContext ceContext)
        {
            if (ceContext.Player == _player)
            {
                DisplayCollisionAmount();
            }
        }
    }
    private void DisplayCollisionAmount()
    {
        _amountCollisions++;

        RpcSetCollisionAmount(_amountCollisions);
        BaseSetCollisionAmount(_amountCollisions);

        SceneDataBus.SendContext(new DashAmount(_amountCollisions));
    }

    private void ProcessContextForPreviousClients(BaseContext context)
    {
        if (context is NewClient ncContext)
        {
            if (ncContext.Client != _player)
            {
                ShowUI();
                RpcSetPlayerNameToUI(_player.Name);
            }
        }
    }
    private void ShowUI()
    {
        RpcSetCollisionAmount(_amountCollisions);
        BaseSetCollisionAmount(_amountCollisions);

        EnableUI();
        RpcEnableUI();
    }

    [ServerCallback]
    private void Update()
    {
        //if (_cameraTarget != null)
        //{
        //    //RpcLookAtTarget(_cameraTarget.Position);
        //    LookAtTarget(_cameraTarget.Position);
        //}
    }

    //[ClientRpc]
    //private void RpcLookAtTarget(Vector3 position) => LookAtTarget(position);
    //private void LookAtTarget(Vector3 position) => transform.LookAt(position);

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
        Debug.Log($"UIPlayerController.Clear");

        _player = null;

        _amountCollisions = 0;
    }
}
