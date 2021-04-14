using UnityEngine;

using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "Inventory/Items/Tool", fileName = "Item")]
public class ToolItemSD : ItemSD 
{
	[ShowIf("isBreakable")]
	[Range(0, 100)]
	[SuffixLabel("%/day")]
	public float decayPerAction = 0f;
}