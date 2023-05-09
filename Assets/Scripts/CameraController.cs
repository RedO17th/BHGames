using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICameraController
{
    ICamera Camera { get; }
}

public class CameraController : BasePlayerController, ICameraController
{
    [SerializeField] private BaseCamera _camera;
    [SerializeField] private float _cameraSpeed;

    public ICamera Camera => _playerCamera;

    private ICameraInput _input = null;
    private ICamera _playerCamera = null;

    private float _xAxis = 0f;
    private float _yAxis = 0f;

    public override void Initialize(IPlayer player)
    {
        _input = GetComponent<ICameraInput>();

        _camera.Initialize();

        _playerCamera = _camera;
    }

    private void Update() => ProcessInputAndRotate();
    private void ProcessInputAndRotate()
    {
        _xAxis += _input.MouseX * _cameraSpeed * Time.deltaTime;
        _yAxis -= _input.MouseY * _cameraSpeed * Time.deltaTime;

        _camera.SetRotation(Quaternion.Euler(_yAxis, _xAxis, 0f));
    }
}
