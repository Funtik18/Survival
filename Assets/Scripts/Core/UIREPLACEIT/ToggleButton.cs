using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.UI;

public class ToggleButton : MonoBehaviour
{
    [SerializeField] private Toggle toggle;
    [SerializeField] private Image icon;

    [SerializeField] private bool isInverse = false;
    private bool IsNormal => isInverse == true;
    private bool IsSelected => isInverse == false;

    [InfoBox("NormalColor", VisibleIf = "IsNormal")]
    [InfoBox("SelectedColor",VisibleIf = "IsSelected")]
    public ColorBlock selected;

    private ColorBlock normal;

    private void Awake()
    {
        toggle.onValueChanged.AddListener(ValueChanged);
        normal = toggle.colors;

        ValueChanged(toggle.isOn);
    }

    private void ValueChanged(bool trigger)
    {
        toggle.colors = trigger ? (isInverse ? normal : selected) : (isInverse ? selected : normal);
    }

    [Button]
    private void Copy()
    {
        if (toggle == null) return;

        selected = toggle.colors;
    }
}