using Mirror;
using UnityEngine;

public interface IDamagable
{
    bool CanDamaged { get; }
    void Damage();
}

public interface IPlayer : IActivatable, IDeactivatable, IMovable, IDamagable
{
    void Initialize();
    T GetController<T>() where T : class;

    void SetPosition(Vector3 position);
}

public class BasePlayer : NetworkBehaviour, IPlayer
{
    [SerializeField] private BasePlayerController[] _controllers;


    public bool CanDamaged => _damageController.CanDamaged;
    public Vector3 Forward => transform.forward;
    public Vector3 Position
    {
        get => transform.position;
        protected set => transform.position = value;
    }


    private IDamageController _damageController = null;

    private CharacterController _controller = null;

    private Vector3 _gravity = new Vector3(0f, -9.8f, 0f);

    public void SetPosition(Vector3 position) => Position = position;

    #region Initialization
    public virtual void Initialize()
    {
        _controller = GetComponent<CharacterController>();

        RpcInitialize();
    }

    [ClientRpc]
    private void RpcInitialize() => BaseInitialize();
    private void BaseInitialize()
    {
        InitializeControllers();
        PrepareControllers();

        _controller = GetComponent<CharacterController>();

        _damageController = GetController<IDamageController>();
    }

    private void InitializeControllers()
    {
        foreach (var controller in _controllers)
            controller.Initialize(this);
    }
    private void PrepareControllers()
    {
        foreach (var controller in _controllers)
        {
            controller.Prepare();
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

    #region Activate
    public virtual void Activate() => RpcActivate();

    [ClientRpc]
    private void RpcActivate() => BaseActivate();
    private void BaseActivate() => EnableControllers();
    private void EnableControllers()
    {
        foreach (var controller in _controllers)
        {
            controller.Enable();
        }
    }
    #endregion

    #region Moving
    public virtual void Rotate(Quaternion rotation)
    {
        BaseRotation(rotation);

        CmdRotation(rotation);
    }
    private void BaseRotation(Quaternion rotation) => transform.rotation = rotation;

    public virtual void Move(Vector3 position)
    {
        CmdMove(position);

        _controller.Move(position);
    }
    public virtual void Dash(Vector3 position)
    {
        CmdDash(position);

        _controller.Move(position);
    }

    [Command]
    private void CmdMove(Vector3 position) => _controller.Move(position);

    [Command]
    private void CmdDash(Vector3 position) => _controller.Move(position);

    [Command]
    private void CmdRotation(Quaternion rotation) => BaseRotation(rotation);

    #endregion

    [Client]
    private void Update() => ProcessGravity();
    private void ProcessGravity()
    {
        _controller?.Move(_gravity);
    }

    #region Deactivate
    public void Damage() => _damageController.Damage();

    public virtual void Deactivate()
    {
        _controller = null;

        RpcDeactivate();
    }

    [ClientRpc]
    private void RpcDeactivate() => BaseDeactivate();
    private void BaseDeactivate()
    {
        DisableControllers();
        DeactivateControllers();

        _damageController = null;
        _controller = null;
    }
    private void DisableControllers()
    {
        foreach (var controller in _controllers)
        {
            controller.Disable();
        }
    }
    private void DeactivateControllers()
    {
        foreach (var controller in _controllers)
        {
            controller.Deactivate();
        }
    }
    #endregion
}
