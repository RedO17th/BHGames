using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
using Input = UnityEngine.Input;

public interface IPlayerDashInput
{
    bool Clicked { get; }
}

public class DashInput : MonoBehaviour, IPlayerDashInput
{
    public bool Clicked => Input.GetMouseButtonDown(1);
}
