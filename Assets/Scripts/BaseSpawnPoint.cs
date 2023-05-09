using UnityEngine;

public class BaseSpawnPoint : MonoBehaviour, ISpawnPoint
{
    public Vector3 Position => transform.position;
}