using UnityEngine;

public interface ICamera
{
    Quaternion YRotation { get; }
    void Initialize();
    void SetRotation(Quaternion rotation);
}
