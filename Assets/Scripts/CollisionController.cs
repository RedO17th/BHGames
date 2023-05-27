using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionController : BasePlayerController
{
    IPlayer _player = null;

    public override void Initialize(IPlayer player)
    {
        _player = player;
    }

    private void OnTriggerEnter(Collider other)
    {
        var player = other?.attachedRigidbody?.GetComponent<IPlayer>();

        if (player != null)
        {
            
        }
    }
}
