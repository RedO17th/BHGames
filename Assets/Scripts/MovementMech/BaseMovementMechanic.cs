using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseMovementMechanic : MonoBehaviour, IEnabable, IDisabable
{
    public abstract void Initialize(IPlayerController controller);

    public virtual void Prepare() { }

    public virtual void Enable() => enabled = true;
    public virtual void Disable() => enabled = false;
}
