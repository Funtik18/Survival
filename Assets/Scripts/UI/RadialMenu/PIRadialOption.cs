using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PIRadialOption : MonoBehaviour
{
    public UnityAction onChoosen;

    [SerializeField] private Pointer pointer;
    [SerializeField] private Image icon;

    [SerializeField] private Image prohibition;

    private bool isProhibition = false;
    public bool IsProhibition
    {
        get => isProhibition;
        set
        {
            isProhibition = value;

            prohibition.enabled = isProhibition;
            pointer.IsEnable = !isProhibition;
        }
    }

    private bool isDisable = false;
    public bool IsDisable
    {
        get => isDisable;
        set
        {
            isDisable = value;

            pointer.IsEnable = !isDisable;
        }
    }

    private void Awake()
    {
        pointer.AddPressListener(Choosen);
    }

    public void Setup(Sprite icon)
    {
        this.icon.sprite = icon;

        UpdateUI();
    }

    public void UpdateUI()
    {
        icon.enabled = icon.sprite != null;
        IsProhibition = isProhibition;
        IsDisable = isDisable;
    }

    public void Choosen()
    {
        onChoosen?.Invoke();
    }
}