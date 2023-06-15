using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public interface IUIPlayerController : IReloadable
{
    IPlayer Player { get; }
}

public class UIPlayerController : BasePlayerController, IUIPlayerController
{
    [SerializeField] private GameObject _canvas;
    [SerializeField] private UICollisionCounter _uiCounter;
    [SerializeField] private TextMeshProUGUI _nameField;

    public IPlayer Player { get; private set; }

    private int _amountCollisions = 0;

    public override void Initialize(IPlayer player)
    {
        if (isServerOnly)
        {
            Player = player;

            _uiCounter.Initialize(this);
            _uiCounter.SetAmount(0);

            _amountCollisions = 0;

            BaseSetPlayerNameToUI(Player.Name);
            RpcSetPlayerNameToUI(Player.Name);
        }
    }

    private void BaseSetPlayerNameToUI(string name)
    {
        _nameField.text = name;
    }

    [ClientRpc]
    private void RpcSetPlayerNameToUI(string name) => BaseSetPlayerNameToUI(name);

    public void Reload() => ResetCounter();
    private void ResetCounter()
    {
        _amountCollisions = 0;

        _uiCounter.SetAmount(_amountCollisions);
        _uiCounter.Enable();
    }

    [Server]
    public override void Enable()
    {
        base.Enable();

        _canvas.SetActive(true);

        _uiCounter.Enable();

        SceneDataBus.OnContextEvent += ProcessContext;
        PlayerDataBus.OnContextEvent += ProcessContext;
    }

    private void ProcessContext(BaseContext context)
    {
        ProcessCollisionContext(context);
        ProcessContextForPreviousClients(context);
    }

    private void ProcessCollisionContext(BaseContext context)
    {
        if (context is CollisionContext ceContext)
        {
            if (ceContext.Player == Player)
            {
                DisplayCollisionAmount();
            }
        }
    }
    
    private void DisplayCollisionAmount()
    {
        _amountCollisions++;

        _uiCounter.SetAmount(_amountCollisions);

        SceneDataBus.SendContext(new DashAmount(_amountCollisions));
    }

    private void ProcessContextForPreviousClients(BaseContext context)
    {
        if (context is NewClient ncContext)
        {
            if (ncContext.Client != Player)
            {
                ShowCounter();
                RpcSetPlayerNameToUI(Player.Name);
            }
        }
    }
    private void ShowCounter()
    {
        _uiCounter.SetAmount(_amountCollisions);
        _uiCounter.Enable();
    }


    [Server]
    public override void Disable()
    {
        base.Disable();

        _canvas.SetActive(false);

        _uiCounter.Disable();

        SceneDataBus.OnContextEvent -= ProcessContext;
        PlayerDataBus.OnContextEvent -= ProcessContext;
    }

    [Server]
    public override void Deactivate()
    {
        _uiCounter.SetAmount(0);
        _uiCounter.Disable();
        _uiCounter.Clear();

        base.Deactivate();
    }

    protected override void Clear()
    {
        Debug.Log($"UIPlayerController.Clear");

        Player = null;

        _amountCollisions = 0;
    }
}
