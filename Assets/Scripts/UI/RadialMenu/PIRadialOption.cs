using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PIRadialOption : MonoBehaviour
{
    public UnityAction onChoosen;

    [SerializeField] private PointerButton button;
    [SerializeField] private Image icon;

    private void Awake()
    {
        button.onPressed = Choosen;
    }

    public void SetIcon(Sprite sprite)
    {
        icon.enabled = sprite == null ? false : true;

        icon.sprite = sprite;
    }

    public void Choosen()
    {
        onChoosen?.Invoke();
    }
}
