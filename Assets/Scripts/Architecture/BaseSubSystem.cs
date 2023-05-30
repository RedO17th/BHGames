using Mirror;
using UnityEngine;

public abstract class BaseSubSystem : MonoBehaviour
{
    public abstract void Initialize(ISceneSystem sceneSystem);

    public virtual void Prepare() { }

    public virtual void Stop() { }

    public virtual void Clear() { }
}
