using UnityEngine.Events;
using UnityEngine;

using Sirenix.OdinInspector;

public class ItemSD : ObjectSD
{
	[PreviewField]
	public Sprite itemSprite;

	[AssetList]
	[InlineEditor(InlineEditorModes.GUIAndPreview)]
	public ItemObject model;

	[Range(0.01f, 99.99f)]
	public float weight = 0.01f;

	public bool isCanned = false;
	public bool isBreakable = true;

	[Min(1)]
	public int stackSize = 1;
}

public enum ItemRarity
{
	Common,
	Rare,
	Epic,
	Legendary,
	Set,
}