using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipt", menuName = "Crafting/CraftingRecipt")]
public class CraftingRecipt : ScriptableObject
{
    public WorkPlace workPlace;

    public List<ItemSD> tools = new List<ItemSD>();

    public Times time;

    public List<CraftingMaterial> materials = new List<CraftingMaterial>();
}
[System.Serializable]
public class CraftingMaterial
{
    public ItemSD data;
    public int count;
}
public enum WorkPlace
{
    None,
    Any,
    WorkBench,
}

//public ItemData yield;
//[TypeFilter("GetFilteredTypeList")]
//public ItemScriptableData data;

//public IEnumerable<Type> GetFilteredTypeList()
//{
//    var q = typeof(ItemScriptableData).Assembly.GetTypes().Where(x => typeof(ItemScriptableData).IsAssignableFrom(x));
//    //q = q.AppendWith(typeof(C1<>).MakeGenericType(typeof(List<float>)));
//    return q;
//}

//[Button]
//public void Check()
//{
//    Debug.LogError(data);
//}