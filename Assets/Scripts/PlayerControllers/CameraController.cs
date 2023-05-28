using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICameraController
{
    ICamera Camera { get; }
}

public class CameraController : BasePlayerController, ICameraController
{
    [SerializeField] private float _cameraSpeed;
    [SerializeField] private BaseCamera _cameraPrefab;
    [SerializeField] private Vector3 _cameraOffset;
    
    public ICamera Camera => _camera;

    private IPlayer _player = null;

    private ICamera _camera = null;
    private ICameraInput _input = null;

    private float _xAxis = 0f;
    private float _yAxis = 0f;

    public override void Initialize(IPlayer player)
    {
        _player = player;

        _input = GetComponent<ICameraInput>();

        _camera = CreateCamera();
        _camera.Initialize();
    }

    private BaseCamera CreateCamera() => Instantiate(_cameraPrefab);

    private void LateUpdate() => ProcessCameraInteraction();
    private void ProcessCameraInteraction()
    {
        Rotate();
        Move();
    }

    private void Move()
    {
        _camera.SetPosition(_cameraOffset + _player.Position);
    }
    private void Rotate()
    {
        _xAxis += _input.MouseX * _cameraSpeed * Time.deltaTime;
        _yAxis -= _input.MouseY * _cameraSpeed * Time.deltaTime;

        _camera.SetRotation(Quaternion.Euler(_yAxis, _xAxis, 0f));
    }

    public override void Deactivate()
    {
        _camera.Destroy();
        _camera = null;

        base.Deactivate();
    }
    protected override void Clear()
    {
        _player = null;
        _input = null;

        _xAxis = 0f;
        _yAxis = 0f;
    }
}
