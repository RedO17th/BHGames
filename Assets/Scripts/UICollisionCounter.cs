using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Mirror;
using System;

public class UICollisionCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;

    public void SetAmount(int amount)
    {
        _text.text = amount.ToString();
    }
}
