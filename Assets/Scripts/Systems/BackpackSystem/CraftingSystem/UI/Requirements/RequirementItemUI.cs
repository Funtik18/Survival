using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RequirementItemUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMPro.TextMeshProUGUI text;

    private BlueprintItem blueprintItem;
    private Item item;

    public void SetRequirement(BlueprintItem blueprintItem, Item item)
    {
        this.blueprintItem = blueprintItem;
        this.item = item;

        UpdateUI();
    }

    private void UpdateUI()
    {
        icon.sprite = blueprintItem.item.itemSprite;
     
        if(item == null)
        {
            text.text = "0 / " + blueprintItem.count + " " + blueprintItem.item.objectName;

            IsReject();
        }
        else
        {
            int stackCount = item.itemData.CurrentStackSize;

            text.text = stackCount + " / " + blueprintItem.count + " " + blueprintItem.item.objectName;

            if(stackCount < blueprintItem.count)
            {
                IsReject();
            }
            else
            {
                IsAccept();
            }
        }
    }

    private void IsAccept()
    {
        icon.color = Color.white;

        text.color = Color.white;
    }
    private void IsReject()
    {
        icon.color = Color.red;

        text.color = Color.red;
    }
}