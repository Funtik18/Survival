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
	public ItemWorldOrientation orientation;

	[Space]
	[Range(0.01f, 99.99f)]
	public float weight = 0.01f;

	[Min(1)]
	public int stackSize = 1;

	[Tooltip("Значит продукт портится - ломается")]
	public bool isBreakable = true;

	[ShowIf("isBreakable")]
	[Range(0, 100)]
	[SuffixLabel("%/day")]
	public float decayOverTime = 0f;
	[ShowIf("isBreakable")]
	[Range(0, 100)]
	[SuffixLabel("%/day")]
	public float decayInside = 0f;
	[ShowIf("isBreakable")]
	[Range(0, 100)]
	[SuffixLabel("%/day")]
	public float decayOutsie = 0f;

	[System.Serializable]
    public class ItemWorldOrientation 
	{
		public Vector3 position = Vector3.zero;
		public Quaternion rotation = Quaternion.identity;
	}
}

public enum ItemRarity
{
	Common,
	Rare,
	Epic,
	Legendary,
	Set,
}