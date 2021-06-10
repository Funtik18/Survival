using Sirenix.OdinInspector;

using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class Overseer : MonoBehaviour
{
    private static Overseer instance;
    public static Overseer Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<Overseer>();
            }
            return instance;
        }
    }

    [InfoBox("Класс отслеживает интерактивные объекты, сохраняет и загружает их состояние.")]

    //public List<AI> AllAI = new List<AI>();

    private List<ItemObject> allInspectedItems = new List<ItemObject>();
    [ReadOnly] [SerializeField] private List<ItemObject> allItems = new List<ItemObject>();

    private List<ContainerObject> allInspectedContainers = new List<ContainerObject>();
    [ReadOnly][SerializeField] private List<ContainerObject> allContainers = new List<ContainerObject>();

    [Title("Zones")]
    [ReadOnly][SerializeField] private List<Generator> zones = new List<Generator>();

    private Transform player;

    private WaitForSeconds seconds;

    private void Awake()
    {
        player = GeneralAvailability.Player.transform;

        seconds = new WaitForSeconds(1);

        StartCoroutine(Oversee());
    }

    public void Subscribe(ContainerObject container)
    {
        if(container != null)
        {
            if (!allInspectedContainers.Contains(container))
            {
                allInspectedContainers.Add(container);
            }
        }
    }
    public void Subscribe(ItemObject item)
    {
        if (item != null)
        {
            if (!allInspectedItems.Contains(item))
            {
                allInspectedItems.Add(item);
            }
        }
    }


    public int IndexOfContainer(ContainerObject reference)
    {
        return allContainers.IndexOf(reference);
    }
    public int IndexOfItem(ItemObject reference)
    {
        return allItems.IndexOf(reference);
    }


    private IEnumerator Oversee()
    {
        while (true)
        {
            for (int i = 0; i < zones.Count; i++)
            {
                zones[i].Behavior(player.position);
            }

            yield return seconds;
        }
    }



    [Button]
    private void UpdateLists()
    {
        allItems = GameObject.FindObjectsOfType<ItemObject>().ToList();
        allContainers = GameObject.FindObjectsOfType<ContainerObject>().ToList();

        zones = GameObject.FindObjectsOfType<Generator>().ToList();
    }



    public void SetData(Data data)
    {
        ItemObject referenceItem = null;
        ItemObject.Data itemData = null;

        ContainerObject referenceContainer = null;
        ContainerObject.Data containerData = null;

        for (int i = 0; i < data.items.Length; i++)
        {
            itemData = data.items[i];
            referenceItem = allItems[itemData.index];

            referenceItem.SetData(itemData);
            Subscribe(referenceItem);
        }

        for (int i = 0; i < data.containers.Length; i++)
        {
            containerData = data.containers[i];
            referenceContainer = allContainers[containerData.index];

            referenceContainer.SetData(containerData);
            Subscribe(referenceContainer);
        }
    }

    public Data GetData()
    {
        ContainerObject.Data[] containers = new ContainerObject.Data[allInspectedContainers.Count];
        ItemObject.Data[] items = new ItemObject.Data[allInspectedItems.Count];

        for (int i = 0; i < allInspectedItems.Count; i++)
        {
            items[i] = allInspectedItems[i].GetData();
        }

        for (int i = 0; i < allInspectedContainers.Count; i++)
        {
            containers[i] = allInspectedContainers[i].GetData();
        }

        Data data = new Data()
        {
            items = items,
            containers = containers,
        };

        return data;
    }

    [System.Serializable]
    public class Data
    {
        public ItemObject.Data[] items;
        public ContainerObject.Data[] containers;
    }
}