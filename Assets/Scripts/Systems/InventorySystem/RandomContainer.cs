using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;
using UnityEditor;

[RequireComponent(typeof(ContainerObject))]
public class RandomContainer : MonoBehaviour
{
    private ContainerObject container;
    private ContainerObject Container 
    {
        get
        {
            if (container == null)
                container = GetComponent<ContainerObject>();
            return container;
        }
    }

    [SerializeField] private bool generateOnAwake = false;
    [Space]
    [SerializeField] private bool useRandomCount = false;
    [ShowIf("useRandomCount")]
    [MinMaxSlider(1, 10)]
    [SerializeField] private Vector2Int randomCount;
    [HideIf("useRandomCount")]
    [Min(0)]
    [SerializeField] private int constCount;

    [Button]
    private void Generate()
    {
        if (useRandomCount)
            constCount = Random.Range(randomCount.x, randomCount.y + 1);

        if (ItemsSD.Instance == null)
            Debug.LogError("Items == null");

        List<ItemSD> items = ItemsSD.Instance.GetRandomContainer(constCount);
        List<ItemDataWrapper> itemsData = new List<ItemDataWrapper>();

        for (int i = 0; i < items.Count; i++)
        {
            ItemDataWrapper itemData = new ItemDataWrapper();

            itemData.scriptableData = items[i];

            itemsData.Add(itemData);
        }

        Container.containerInventory.initItems = itemsData;
    }
    [Button]
    private void Clear()
    {
        Container.containerInventory.initItems.Clear();
    }

    private void Awake()
    {
        if (generateOnAwake)
            Generate();
    }
}