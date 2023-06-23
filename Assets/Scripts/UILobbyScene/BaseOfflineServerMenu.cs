using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class BaseOfflineServerMenu : MonoBehaviour
{
    [SerializeField] private TMP_InputField _adress;
    [SerializeField] private Button _runBtn;

    public event Action<string> OnStartBynetAdress;

    public void Enable()
    {
        gameObject.SetActive(true);

        _runBtn.onClick.AddListener(ProcessConnectionToserver);
    }

    private void ProcessConnectionToserver()
    {
        if(_adress.text != string.Empty)
            OnStartBynetAdress.Invoke(_adress.text);
    }

    public void Disable()
    {
        gameObject.SetActive(false);

        _runBtn.onClick.RemoveListener(ProcessConnectionToserver);
    }
}
