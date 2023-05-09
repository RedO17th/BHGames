using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCamera : MonoBehaviour, ICamera
{
    public Quaternion YRotation
    {
        get
        { 
            return new Quaternion(0f, _transform.rotation.y, 0f, 0f);
        }
    }

    private Transform _transform = null;

    public virtual void Initialize()
    {
        _transform = transform;
    }

    public virtual void SetRotation(Quaternion rotation)
    {
        _transform.rotation = rotation;
    }
}
