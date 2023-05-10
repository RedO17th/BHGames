using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovementInput
{
    Vector3 Direction { get; }
}

public class MovementInput : MonoBehaviour, IMovementInput
{
    public Vector3 Direction
    {
        get
        {
            return new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized;
        }
    }
}
