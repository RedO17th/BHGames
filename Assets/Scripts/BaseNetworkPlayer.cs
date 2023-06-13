using Mirror;
using System;
using UnityEngine;
using UnityEngine.UIElements;

public interface IDamagable
{
    bool CanDamaged { get; }
    void Damage();
}

public interface IPlayer : IEnabable, IDisabable, IDeactivatable, IMovable, IDamagable
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

    public bool CanDamaged => _damageController.CanDamaged;
    public Vector3 Forward => transform.forward;
    public Vector3 Position
    {
        get => transform.position;
        protected set => transform.position = value;
    }


    private IDamageController _damageController = null;

    private Vector3 _gravity = new Vector3(0f, -9.8f, 0f);

    public void SetPosition(Vector3 position) => Position = position;

    #region Systemic
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

    //Убрать
    public override void OnStartClient()
    {
        base.OnStartClient();

        DontDestroyOnLoad(gameObject);
    }
    public override void OnStartServer()
    {
        base.OnStartServer();

        DontDestroyOnLoad(gameObject);
    }
    //..

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

    [ClientRpc]
    private void RpcDash(Vector3 position) => BaseDash(position);
    private void BaseDash(Vector3 position) => _controller.Move(position);
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
