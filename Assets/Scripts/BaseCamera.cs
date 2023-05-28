using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BaseCamera : MonoBehaviour, ICamera
{
    public Vector3 Position => _transform.position;
    public Quaternion YRotation
    {
        get => new Quaternion(0f, _transform.rotation.y, 0f, _transform.rotation.w);
    }

    private Transform _transform = null;

    public virtual void Initialize()
    {
        _transform = GetComponent<Transform>();
    }

    public virtual void SetPosition(Vector3 position)
    {
        _transform.position = position;
    }
    public virtual void SetRotation(Quaternion rotation)
    {
        _transform.rotation = rotation;
    }

    public void Destroy()
    {
        Destroy(gameObject, 0.5f);
    }
}
