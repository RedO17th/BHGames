using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialController : BasePlayerController
{
    [SerializeField] private SkinnedMeshRenderer _renderer;
    [SerializeField] private Material _material;

    private Material _sourceMaterial = null;

    private bool _check = false;

    public override void Initialize(IPlayer player)
    {
        _sourceMaterial = _renderer.material;
    }

    public override void Prepare() { }

    //public virtual void Enable() => enabled = true;
    //public virtual void Disable() => enabled = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (_check == false)
            {
                _check = true;
                _renderer.material = _material;
            }
            else
            {
                _check = false;
                _renderer.material = _sourceMaterial;
            }
        }
    }

    //public virtual void Deactivate() => Clear();
    protected override void Clear() { }
}
