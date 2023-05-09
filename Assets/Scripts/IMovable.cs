using UnityEngine;

public interface IMovable
{
    void Rotate(Quaternion rotation);
    void Move(Vector3 position);
}
