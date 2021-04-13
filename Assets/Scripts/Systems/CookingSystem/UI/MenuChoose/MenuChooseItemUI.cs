using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MenuChooseItemUI : MonoBehaviour
{
    public UnityAction<MenuChooseItemUI> onChoosen;

    [SerializeField] private Toggle toggle;
    [SerializeField] private Image icon;

    public Item item;

    private void Awake()
    {
        toggle.onValueChanged.AddListener(Choosen);
    }

    public void Setup(Item item)
    {
        this.item = item;

        UpdateUI();
    }
    private void UpdateUI()
    {
        icon.sprite = item.itemData.scriptableData.itemSprite;
    }

    private void Choosen(bool value)
    {
        if(value == true)
        {
            onChoosen?.Invoke(this);
        }
    }
}