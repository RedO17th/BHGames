using UnityEngine;

public interface ISpawnPoint
{ 
    bool IsBusy { get; }
    Vector3 Position { get; }
    void SetBusyState();
}
