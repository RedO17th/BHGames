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
    [SerializeField] private TMP_InputField _playerName;
    [SerializeField] private TMP_InputField _adress;
    [SerializeField] private Button _connectBtn;

    public event Action<ClientLoggin> OnClientConnect;

    public void Enable()
    {
        gameObject.SetActive(true);

        _connectBtn.onClick.AddListener(ProcessConnectionToserver);
    }

    private void ProcessConnectionToserver()
    {
        var adress = _adress.text;
        var name = _playerName.text;

        if (adress != string.Empty && name != string.Empty)
        {
            OnClientConnect.Invoke(new ClientLoggin(adress, name));
        }
    }

    public void Disable()
    {
        gameObject.SetActive(false);

        _connectBtn.onClick.RemoveListener(ProcessConnectionToserver);
    }
}
