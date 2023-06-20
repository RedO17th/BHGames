using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClientLoggin
{
    public string Adress { get; private set; }
    public string Name { get; private set; }

    public ClientLoggin(string adress, string name)
    {
        Adress = adress;
        Name = name;
    }
}

public class BaseClientPanel : MonoBehaviour
{
    [SerializeField] private TMP_InputField _adress;
    [SerializeField] private Button _connectBtn;

    public event Action<string> OnStartBynetAdress;

    public void Enable()
    {
        gameObject.SetActive(true);

        _connectBtn.onClick.AddListener(ProcessConnectionToserver);
    }

    private void ProcessConnectionToserver()
    {
        if (_adress.text != string.Empty)
            OnStartBynetAdress.Invoke(_adress.text);
    }

    public void Disable()
    {
        gameObject.SetActive(false);

        _connectBtn.onClick.RemoveListener(ProcessConnectionToserver);
    }
}
