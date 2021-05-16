using UnityEngine;

using Sirenix.OdinInspector;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Game/Container", fileName = "Data")]
public class ContainerSD : ObjectSD
{
    [HideLabel]
    public Container container;
}
[System.Serializable]
public class Container 
{
    [InfoBox("Items которые точно будут в контейнере")]
    public List<ItemDataRandom> staticItems = new List<ItemDataRandom>();

    [MinMaxSlider(0, 12)]
    public Vector2Int itemsCount;
    [InfoBox("Items которые динамичные могут появится, а могут не появится")]
    public List<ItemDataRandom> dinamicItems = new List<ItemDataRandom>();
    [Tooltip("Вычесть dinamicItems из staticItems если таковы имеются")]
    public bool useDiff = false;

    [ReadOnly] [SerializeField] private List<ItemSD> posibleItems = new List<ItemSD>();
}
