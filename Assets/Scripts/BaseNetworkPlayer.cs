using Mirror;
using System;
using UnityEngine;
using UnityEngine.UIElements;

public interface INamable
{ 
    string Name { get; }
    void SetName(string name);
}

public interface IReloadable
{
    void Reload();
}

public interface IDamagable
{
    bool CanDamaged { get; }
    void Damage();
}

public interface IPlayer : INamable, IEnabable, IDisabable, IDeactivatable, IMovable, IDamagable, IReloadable
{
    void Initialize();
    T GetController<T>() where T : class;

    void SetPosition(Vector3 position);

    void Remove();
}

public class BaseNetworkPlayer : NetworkBehaviour, IPlayer
{
    [SerializeField] private CharacterController _controller = null;

    [SerializeField] private BasePlayerController[] _controllers;

    public string Name { get; protected set; }
    public bool CanDamaged => _damageController.CanDamaged;
    public Vector3 Forward => transform.forward;
    public Vector3 Position
    {
        get => transform.position;
        protected set => SetPositionWithoutCharController(value);
    }
    private void SetPositionWithoutCharController(Vector3 position)
    {
        _controller.enabled = false;
        transform.position = position;
        _controller.enabled = true;
    }

    private IDamageController _damageController = null;

    private Vector3 _gravity = new Vector3(0f, -9.8f, 0f);

    #region Systemic
    public void SetName(string name)
    {
        RpcSetName(name);
        BaseSetName(name);
    }

    [ClientRpc]
    private void RpcSetName(string name) => BaseSetName(name);
    private void BaseSetName(string name) => Name = name;

    public virtual void SetPosition(Vector3 position)
    {
        RpcSetPosition(position);
        BaseSetPosition(position);
    }

    [ClientRpc]
    private void RpcSetPosition(Vector3 position) => BaseSetPosition(position);
    private void BaseSetPosition(Vector3 position) => Position = position;

    public virtual void Reload()
    {
        foreach (var controller in _controllers)
        {
            if (controller is IReloadable rController)
            {
                rController.Reload();
            }
        }
    }

    public virtual T GetController<T>() where T : class
    {
        T result = null;

        foreach (var controller in _controllers)
        {
            if (controller is T mech)
            {
                result = mech;
                break;
            }
        }

        return result;
    }
    #endregion

    #region Initialization
    public virtual void Initialize()
    {
        RpcInitializeControllers();
        BaseInitializeControllers();

        RpcPrepareControllers();
        BasePrepareControllers();

        _damageController = GetController<IDamageController>();
    }

    [ClientRpc]
    private void RpcInitializeControllers() => BaseInitializeControllers();
    private void BaseInitializeControllers()
    {
        foreach (var controller in _controllers)
            controller.Initialize(this);
    }

    [ClientRpc]
    private void RpcPrepareControllers() => BasePrepareControllers();
    private void BasePrepareControllers()
    {
        foreach (var controller in _controllers)
        {
            controller.Prepare();
        }
    }
    #endregion

    #region Enable
    public void Enable() => EnableControllers();
    private void EnableControllers()
    {
        foreach (var controller in _controllers)
        {
            controller.Enable();
        }
    }
    #endregion

    #region Moving

    [Server]
    public virtual void Rotate(Quaternion rotation)
    {
        BaseRotate(rotation);
        RpcRotate(rotation);
    }
    private void BaseRotate(Quaternion rotation) => transform.rotation = rotation;

    [ClientRpc]
    private void RpcRotate(Quaternion rotation) => BaseRotate(rotation);

    [Server]
    public virtual void Move(Vector3 position)
    {
        BaseMove(position);
        RpcMove(position);
    }
    private void BaseMove(Vector3 position) => _controller.Move(position);

    [ClientRpc]
    private void RpcMove(Vector3 position) => BaseMove(position);

    [Server]
    public virtual void Dash(Vector3 position)
    {
        RpcDash(position);
        BaseDash(position);
    }
    private void BaseDash(Vector3 position) => _controller.Move(position);

    [ClientRpc]
    private void RpcDash(Vector3 position) => BaseDash(position);

    #endregion

    [ServerCallback]
    private void Update() => ProcessGravity();
    private void ProcessGravity() => _controller.Move(_gravity);

    #region Damage
    public void Damage() => _damageController.Damage();
    #endregion

    #region Disable
    public void Disable() => DisableControllers();
    private void DisableControllers()
    {
        foreach (var controller in _controllers)
        {
            controller.Disable();
        }
    }
    #endregion

    public virtual void Deactivate()
    {
        DeactivateControllers();

        _damageController = null;
        _controller = null;
    }

    private void DeactivateControllers()
    {
        foreach (var controller in _controllers)
        {
            controller.Deactivate();
        }
    }

    public void Remove()
    {
        Destroy(gameObject);

        RpcRemove();
    }

    [ClientRpc]
    private void RpcRemove() => Destroy(gameObject);
}
