using UnityEngine;

public class BaseSpawnPoint : MonoBehaviour, ISpawnPoint
{
    public bool IsBusy { get; protected set; } = false;
    public Vector3 Position => transform.position;

    public virtual void SetBusyState() => IsBusy = true;
}