using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSubSystem : MonoBehaviour
{
    public abstract void Initialize(ISceneSystem sceneSystem);

    public virtual void Prepare() { }
    public virtual void Clear() { }
}
