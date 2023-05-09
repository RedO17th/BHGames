using UnityEngine;

public interface IMovable : IDashable
{
    void Rotate(Quaternion rotation);
    void Move(Vector3 position);
}
