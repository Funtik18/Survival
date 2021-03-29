using UnityEngine;

using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "Inventory/Item", fileName = "Item")]
public class ItemScriptableData : ScriptableObject
{
	[HideLabel]
	public ItemData information;
}

[System.Serializable]
public class ItemData
{
	public string name;

	[PreviewField]
	public Sprite itemSprite;

	[AssetList]
	[InlineEditor(InlineEditorModes.GUIAndPreview)]
	public ItemObject model;

	[TextArea(5, 5)]
	public string description;

	public bool isStackable = false;
	[ShowIf("isStackable")]
	[Min(0)]
	public int maxStackSize = 1;
}

public class Item
{
	public System.Guid ID { get; protected set; }
	public ItemScriptableData ScriptableItem { get; protected set; }
	public ItemData ItemData => ScriptableItem.information;

	public Item(ItemScriptableData data)
    {
		ID = System.Guid.NewGuid();
		ScriptableItem = data;
	}
}