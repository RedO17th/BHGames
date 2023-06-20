using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class UINetworkClientMenu : MonoBehaviour
{
    [SerializeField] private TMP_InputField _clientName;
    [SerializeField] private Button _addBtn;

    public event Action<string> OnAddClientInfo;

    public void Enable()
    {
        gameObject.SetActive(true);

        _addBtn.onClick.AddListener(ProcessConnectionToserver);
    }

    private void ProcessConnectionToserver()
    {
        if (_clientName.text != string.Empty)
            OnAddClientInfo.Invoke(_clientName.text);
    }

    public void Disable()
    {
        gameObject.SetActive(false);

        _addBtn.onClick.RemoveListener(ProcessConnectionToserver);
    }
}
