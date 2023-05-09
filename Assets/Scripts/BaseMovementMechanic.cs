using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseMovementMechanic : MonoBehaviour
{
    public abstract void Initialize(IPlayerController controller);
}
