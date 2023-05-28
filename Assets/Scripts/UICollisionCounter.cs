using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UICollisionCounter : MonoBehaviour, IEnabable, IDisabable
{
    [SerializeField] private TextMeshProUGUI _text;

    private ICollisionCounterController _counter = null;

    private ICamera _cameraTarget = null;

    public void Initialize(ICollisionCounterController counter)
    {
        _counter = counter;
        _cameraTarget = GetCamera();
    }

    private ICamera GetCamera()
    {
        return _counter.Player.GetController<ICameraController>().Camera;
    }

    public void SetAmount(int amount) => _text.text = amount.ToString();

    public void Enable() => enabled = true;
    public void Disable() => enabled = false;

    private void Update()
    {
        if (_cameraTarget != null)
        { 
            transform.LookAt(_cameraTarget.Position);
        }
    }

    public void Clear()
    {
        _cameraTarget = null;
        _counter = null;
    }
}
