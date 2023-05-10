using UnityEngine;

public interface ICamera
{
    Quaternion YRotation { get; }
    void Initialize();
    void SetPosition(Vector3 position);
    void SetRotation(Quaternion rotation);
}
