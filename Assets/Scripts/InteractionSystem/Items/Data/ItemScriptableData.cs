using UnityEngine;

using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "Inventory/Item", fileName = "Item")]
public class ItemScriptableData : ObjectScriptableData
{
	[PreviewField]
	public Sprite itemSprite;

	[AssetList]
	[InlineEditor(InlineEditorModes.GUIAndPreview)]
	public ItemObject model;

	public bool isStackable = false;
	[ShowIf("isStackable")]
	[Min(0)]
	public int maxStackSize = 1;
}
public class Item
{
	public System.Guid ID { get; protected set; }
	public ItemScriptableData ScriptableItem { get; protected set; }

	public Item(ItemScriptableData data)
    {
		ID = System.Guid.NewGuid();
		ScriptableItem = data;
	}
}