using Mirror;

public abstract class BaseNetworkSubSystem : NetworkBehaviour
{
    public abstract void Initialize(ILobbyNetManager lobbyManager);

    public virtual void Prepare() { }

    public virtual void Stop() { }

    public virtual void Clear() { }
}
