using UnityEngine;

using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "Inventory/Item", fileName = "Item")]
public class ItemScriptableData : ScriptableObject
{
	[HideLabel]
	public ItemData data;
}
[System.Serializable]
public class ItemData
{
	public string name;

	[PreviewField]
	public Sprite itemSprite;

	[AssetList]
	[InlineEditor(InlineEditorModes.GUIAndPreview)]
	public ItemModel model;

	[TextArea(5, 5)]
	public string description;
}