using Sirenix.OdinInspector;

using UnityEngine;

public class ClothingItemSD : ItemSD
{
	[ShowIf("isBreakable")]
	[Range(0, 100)]
	[SuffixLabel("%/day")]
	public float decayStored = 0f;
}
