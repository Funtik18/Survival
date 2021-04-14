using DanielLochner.Assets.SimpleScrollSnap;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MenuChoose : MonoBehaviour
{
    public UnityAction<Item> onChoosen;

    [SerializeField] private MenuChooseItemUI itemPrefab;
    [SerializeField] private ToggleGroup toggleGroup;

    [SerializeField] private SimpleScrollSnap itemsScroll;

    private List<MenuChooseItemUI> menus = new List<MenuChooseItemUI>();

    private void Awake()
    {
        itemsScroll.onPanelSelected.AddListener(Choosen);
    }

    public void Setup(List<Item> items)
    {
        Clear();

        for (int i = 0; i < items.Count; i++)
        {
            MenuChooseItemUI item = Instantiate(itemPrefab);
            item.toggle.group = toggleGroup;
            item.Setup(items[i], Choosen);

            menus.Add(item);
        }

        for (int i = 0; i < menus.Count; i++)
        {
            Transform t = menus[i].transform;

            itemsScroll.AddToBack(t);

            t.localScale = Vector3.one;
        }

        if (menus.Count > 0)
            Choosen(menus[0]);
    }

    public void Clear()
    {
        menus.Clear();

        while (itemsScroll.NumberOfPanels != 0)
        {
            itemsScroll.RemoveFromBack();
        }
    }

    private void Choosen(MenuChooseItemUI ui)
    {
        //interactiable добавить выключение активного тогла
        itemsScroll.targetPanel = menus.IndexOf(ui);

        onChoosen?.Invoke(ui.item);
    }
    private void Choosen()
    {
        if(menus.Count > 0)
        {
            onChoosen?.Invoke(menus[itemsScroll.targetPanel].item);
        }
    }
}
