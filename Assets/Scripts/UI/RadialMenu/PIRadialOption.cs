using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PIRadialOption : MonoBehaviour
{
    public UnityAction onChoosen;

    [HideInInspector] public RadialOptionData Data { get; private set; }

    [SerializeField] private PointerButton button;
    [SerializeField] private Image icon;

    private void Awake()
    {
        button.AddPressListener(Choosen);
    }

    public void SetData(RadialOptionData data)
    {
        this.Data = data;

        UpdateUI();
    }

    private void UpdateUI()
    {
        Sprite sprite = Data == null ? null : Data.scriptableData.optionIcon;
        icon.enabled = sprite == null ? false : true;
        icon.sprite = sprite;
    }

    public void Choosen()
    {
        onChoosen?.Invoke();
        Data.EventInvoke();
    }
}
