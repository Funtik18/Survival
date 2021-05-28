using UnityEngine;
using UnityEngine.UI;

public class HarvestingResultUI : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMPro.TextMeshProUGUI itemCount;
    [SerializeField] private TMPro.TextMeshProUGUI itemName;

    private ItemYield item;

    public void SetItem(ItemYield item)
    {
        this.item = item;

        UpdateUI();
    }
    private void UpdateUI()
    {
        itemIcon.sprite = item.item.scriptableData.itemSprite;
        itemCount.text = item.isRandom ? "x" + item.item.CurrentStackSize + "-" + item.maxStackSize : "x" + item.item.CurrentStackSize;
        itemName.text = item.item.scriptableData.objectName;
    }
}