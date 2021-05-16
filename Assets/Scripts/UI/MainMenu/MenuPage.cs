using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MenuPage : MonoBehaviour
{
    public UnityAction<MenuPage> onOpened;
    public UnityAction<MenuPage> onClossed;

    public void Open()
    {
        gameObject.SetActive(true);
    }
    public void OpenPage()
    {
        Open();
        onOpened?.Invoke(this);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
    public void ClosePage()
    {
        Close();
        onClossed?.Invoke(this);
    }
}
