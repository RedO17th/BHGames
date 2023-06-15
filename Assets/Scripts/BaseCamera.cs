using Mirror;
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

    private Vector3 _sourcePosition = Vector3.zero;
    private Quaternion _sourceRotation = Quaternion.identity;

    public virtual void Initialize()
    {
        _transform = GetComponent<Transform>();

        _sourcePosition = _transform.position;
        _sourceRotation = _transform.rotation;
    }

    public void Reload() => RpcReload();

    //[ClientRpc]
    private void RpcReload()
    {
        _transform.rotation = _sourceRotation;
        _transform.position = _sourcePosition;
    }

    public virtual void SetPosition(Vector3 position)
    {
        _transform.position = position;
    }
    public virtual void SetRotation(Quaternion rotation)
    {
        _transform.rotation = rotation;
    }

    public void Destroy() => Destroy(gameObject, 0.5f);


}
