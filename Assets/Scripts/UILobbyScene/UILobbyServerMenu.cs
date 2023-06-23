using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILobbyServerMenu : MonoBehaviour, IEnabable, IDisabable
{
    public void Enable()
    {
        Debug.Log($"UILobbyServerMenu.Enable");

        gameObject.SetActive(true);
    }

    public void Disable()
    {
        Debug.Log($"UILobbyServerMenu.Disable");

        gameObject.SetActive(false);
    }
}
