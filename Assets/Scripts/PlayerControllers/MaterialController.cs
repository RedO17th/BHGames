using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MaterialController : BasePlayerController, IReloadable
{
    [SerializeField] private SkinnedMeshRenderer _renderer;
    [SerializeField] private Material _material;
    [SerializeField] private Material _sourceMaterial;

    private IPlayer _player = null;

    public override void Initialize(IPlayer player)
    {
        _player = player;
    }

    public override void Prepare() { }

    public void Reload() => SetMaterial(false);

    [Server]
    public override void Enable()
    {
        base.Enable();

        PlayerDataBus.OnContextEvent += ProcessContext;
    }

    [Server]
    private void ProcessContext(BaseContext context)
    {
        if (context is DamageContext dContext)
        {
            if (dContext.Player == _player)
            {
                ProcessDamageContext(switchMaterial: dContext.Begin);
            }
        }
    }

    private void ProcessDamageContext(bool switchMaterial) => SetMaterial(switchMaterial);
    private void SetMaterial(bool switchMaterial)
    {
        BaseSetMaterial(switchMaterial);

        RpcSetMaterial(switchMaterial);
    }

    private void BaseSetMaterial(bool switchMaterial)
    {
        _renderer.material = (switchMaterial) ? _material : _sourceMaterial;
    }

    [ClientRpc]
    private void RpcSetMaterial(bool switchMaterial) => BaseSetMaterial(switchMaterial);

    [Server]
    public override void Disable()
    {
        PlayerDataBus.OnContextEvent -= ProcessContext;

        base.Disable();
    }

    [Server]
    public override void Deactivate()
    {
        SetMaterial(false);

        base.Deactivate();
    }
    protected override void Clear()
    {
        BaseClear();
        RpcClear();
    }

    private void BaseClear()
    {
        _player = null;
    }

    [ClientRpc]
    private void RpcClear() => BaseClear();


}
