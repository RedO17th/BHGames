using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRunInput
{
    bool IsRunning { get;}
}

public class RunInput : MonoBehaviour, IRunInput
{
    public bool IsRunning => Input.GetKey(KeyCode.LeftShift);
}
