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

    [SerializeField] private SimpleScrollSnap itemsScroll;

    private List<MenuChooseItemUI> menus = new List<MenuChooseItemUI>();

    public void Setup(List<Item> items)
    {
        DisposeScroll();

        for (int i = 0; i < items.Count; i++)
        {
            MenuChooseItemUI item = Instantiate(itemPrefab);
            item.onChoosen += Choosen;
            item.Setup(items[i]);

            menus.Add(item);

            itemsScroll.AddToBack(item.transform);
            item.transform.localScale = Vector3.one;
        }
    }

    private void DisposeScroll()
    {
        menus.Clear();

        while (itemsScroll.NumberOfPanels != 0)
        {
            itemsScroll.RemoveFromBack();
        }
    }

    private void Choosen(MenuChooseItemUI ui)
    {
        onChoosen?.Invoke(ui.item);
    }
}
