using UnityEngine;

public abstract class EffectSD : ScriptableObject
{
	public Sprite effectIcon;
	public string effectName = "Effect";

	public abstract Effect GetEffect();
}