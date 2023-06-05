using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseMovementMechanic : NetworkBehaviour, IEnabable, IDisabable, IDeactivatable
{
    public abstract void Initialize(IPlayerController controller);

    public virtual void Prepare() { }

    public virtual void Enable() => enabled = true;
    public virtual void Disable() => enabled = false;

    public virtual void Deactivate() => Clear();
    protected virtual void Clear() { }
}
