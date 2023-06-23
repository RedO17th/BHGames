using Mirror;

public abstract class BaseNetworkSubSystem : NetworkBehaviour
{
    public abstract void Initialize(ILobbyNetManager lobbyManager);

    public virtual void SubScribe() { }

    //[TODO] Обдумать появление метода, который получает данные из других сущностей после их инициализации 

    public virtual void StartSystem() { }
    public virtual void StopSystem() { }

    public virtual void UnSubScribe() { }

    public virtual void Clear() { }
}
