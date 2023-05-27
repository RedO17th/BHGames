using UnityEngine;

public interface ICamera
{
    Vector3 Position { get; }
    Quaternion YRotation { get; }
    void Initialize();
    void SetPosition(Vector3 position);
    void SetRotation(Quaternion rotation);
}
