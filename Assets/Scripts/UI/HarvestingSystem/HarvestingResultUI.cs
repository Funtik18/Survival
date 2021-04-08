using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HarvestingResultUI : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMPro.TextMeshProUGUI itemCount;
    [SerializeField] private TMPro.TextMeshProUGUI itemName;

    private ItemData item;

    public void SetItem(ItemData item)
    {
        this.item = item;

        UpdateUI();
    }
    private void UpdateUI()
    {
        itemIcon.sprite = item.scriptableData.itemSprite;
        itemCount.text = "x" + item.StackSize;
        itemName.text = item.scriptableData.objectName;
    }
}