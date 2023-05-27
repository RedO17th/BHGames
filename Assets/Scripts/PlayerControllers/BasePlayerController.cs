using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerController 
{
    void Initialize(IPlayer player);
}

public abstract class BasePlayerController : MonoBehaviour, IPlayerController, IEnabable, IDisabable
{
    public abstract void Initialize(IPlayer player);

    public virtual void Enable() => enabled = true;
    public virtual void Disable() => enabled = false;
}
