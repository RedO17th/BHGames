using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialController : BasePlayerController
{
    [SerializeField] private SkinnedMeshRenderer _renderer;
    [SerializeField] private Material _material;

    private IPlayer _player = null;

    private Material _sourceMaterial = null;

    public override void Initialize(IPlayer player)
    {
        _player = player;
        _sourceMaterial = _renderer.material;
    }

    public override void Prepare() { }

    public override void Enable()
    {
        base.Enable();

        PlayerDataBus.OnContextEvent += ProcessContext;
    }

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

    private void ProcessDamageContext(bool switchMaterial)
    {
        var material = (switchMaterial) ? _material : _sourceMaterial;

        SetMaterial(material);
    }

    private void SetMaterial(Material material) => _renderer.material = material;

    public override void Disable()
    {
        PlayerDataBus.OnContextEvent -= ProcessContext;

        base.Disable();
    }    

    public override void Deactivate()
    {
        SetMaterial(_sourceMaterial);

        base.Deactivate();
    }
    protected override void Clear()
    {
        _sourceMaterial = null;
        _player = null;
    }
}
