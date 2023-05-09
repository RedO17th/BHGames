using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerController 
{
    void Initialize(IPlayer player);
}

public abstract class BasePlayerController : MonoBehaviour, IPlayerController
{
    public abstract void Initialize(IPlayer player);
}
