using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovementInput
{
    float Forward { get; }
    float Side { get; }
}

public class MovementInput : MonoBehaviour, IMovementInput
{
    public float Forward => Input.GetAxis("Vertical");
    public float Side => Input.GetAxis("Horizontal");
}
