using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

public class FireMenu : WindowUI
{
    public UnityAction<Item> onUpdated;

    [SerializeField] private MenuChoose menu;
    [SerializeField] private TMPro.TextMeshProUGUI itemName;
    [SerializeField] private TMPro.TextMeshProUGUI itemAdditional;

    private void Awake()
    {
        menu.onChoosen += Choosen;
    }

    public void Setup(List<Item> items)
    {
        menu.Setup(items);
    }

    public void Clear()
    {
        menu.Clear();
    }

    private void Choosen(Item item)
    {
        itemName.text = item.itemData.scriptableData.objectName;

        if(item.itemData.scriptableData is ToolContainerItemSD containerItemSD)
        {
            itemAdditional.text = "Volume : " + containerItemSD.volume + " L";
        }
        else if(item.itemData.scriptableData is WaterItemSD waterItem)
        {
            itemAdditional.text = "Weight : " + item.itemData.CurrentBaseWeight + " L";
        }
        else
        {
            itemAdditional.text = "";
        }

        onUpdated?.Invoke(item);
    }
}
