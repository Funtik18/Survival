using UnityEngine.Events;
using UnityEngine;

using Sirenix.OdinInspector;

public abstract class ItemSD : ObjectSD
{
	[PreviewField]
	public Sprite itemSprite;

	[AssetList]
	[InlineEditor(InlineEditorModes.GUIAndPreview)]
	public ItemObject model;
	[Space]
	public Vector3 viewPosition = Vector3.zero;
	public Quaternion viewRotation = new Quaternion(0, 0, 0, 1);
	[Space]

	[Range(0.01f, 99.99f)]
	public float weight = 0.01f;

	public bool isCanned = false;
	[Tooltip("Значит продукт портится - ломается")]
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