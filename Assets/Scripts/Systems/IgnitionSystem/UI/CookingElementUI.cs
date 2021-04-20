using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CookingElementUI : MonoBehaviour
{
    public UnityAction onChoosen;

    [SerializeField] private Pointer pointer;
    [Space]
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMPro.TextMeshProUGUI itemName;
    [SerializeField] private TMPro.TextMeshProUGUI itemDurability;

    private Item item;

    private void Awake()
    {
        pointer.onPressed.AddListener(Choosen);
    }

    public void Setup(Item item)
    {
        this.item = item;

        UpdateUI();
    }
    private void UpdateUI()
    {
        itemIcon.sprite = item.itemData.scriptableData.itemSprite;
        itemName.text = item.itemData.scriptableData.objectName;
        itemDurability.text = item.itemData.CurrentDurrability + "%";
    }

    private void Choosen()
    {
        onChoosen?.Invoke();
    }
}
