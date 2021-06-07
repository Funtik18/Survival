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

	[HideIf("isInfinityStack")]
	public bool isInfinityWeight = false;
	[HideIf("isInfinityWeight")]
	public bool isInfinityStack = false;

	[SuffixLabel("kg", true)]
	[ShowIf("IsWeight")]
	[Space]
	[Range(0.01f, 99.99f)]
	public float weight = 0.01f;
	[Tooltip("Formule : CurrentStackSize * weight")]
	public bool isWeightDependesStack = true;

	[ShowIf("IsStack")]
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


	[TabGroup("RANDOM")]
	public bool isCanRandomStack = false;
	[TabGroup("RANDOM")]
	public bool isCanRandomWeight = false;


	[System.Serializable]
    public class ItemWorldOrientation 
	{
		public Vector3 position = Vector3.zero;
		public Quaternion rotation = Quaternion.identity;
	}

	private bool IsWeight => !isInfinityWeight;
	private bool IsStack => !isInfinityStack && !isInfinityWeight;
}

public enum ItemRarity
{
	Common,
	Rare,
	Epic,
	Legendary,
	Set,
}