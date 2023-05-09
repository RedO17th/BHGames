using UnityEngine;

public interface IMovable : IDashable
{
    Vector3 Position { get; }
    Vector3 Forward { get; }

    void Rotate(Quaternion rotation);
    void Move(Vector3 position);
}
