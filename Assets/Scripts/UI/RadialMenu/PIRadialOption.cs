using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PIRadialOption : MonoBehaviour
{
    public UnityAction onChoosen;

    [HideInInspector] public RadialOptionData Data { get; private set; }

    [SerializeField] private PointerButton button;
    [SerializeField] private Image icon;

    [SerializeField] private Image prohibition;

    private void Awake()
    {
        button.AddPressListener(Choosen);
    }

    public void SetData(RadialOptionData data)
    {
        this.Data = data;

        UpdateUI();
    }

    public void UpdateUI()
    {
        if( Data != null)
        {
            Sprite sprite = Data.scriptableData.optionIcon;
            icon.enabled = sprite == null ? false : true;
            icon.sprite = sprite;
            if (Data.scriptableData is RadialOptionBuildingSD buildingSD)
            {
                BuildingObject obj = buildingSD.building;

                bool isProhibition = obj == null ? false : !obj.IsCanBeBuild;
                prohibition.enabled = isProhibition;
                button.IsEnable = !isProhibition;
            }
        }
        else
        {
            icon.enabled = false;
            prohibition.enabled = false;
            button.IsEnable = false;
        }
    }

    public void Choosen()
    {
        onChoosen?.Invoke();
        Data.EventInvoke();
    }
}
