using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : PickableItem
{
    public ItemData data;
}
[System.Serializable]
public class ItemData
{
	public string name;
	[TextArea]
	public string description;
}