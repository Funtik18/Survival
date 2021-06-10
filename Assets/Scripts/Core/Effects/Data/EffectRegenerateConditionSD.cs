using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "Game/Effects/Regenerate Condition", fileName = "Effect")]
public class EffectRegenerateConditionSD : EffectSD
{
	public float healAmount = 10f;

	public Effect.EffectAction duration;

	[ShowInInspector]
	[ReadOnly] private string Result => healAmount * (duration.actionAmount/duration.actionStep) + " for " + duration.actionDelay * (duration.actionAmount / duration.actionStep) + "sec";

	public override Effect GetEffect() => new EffectRegenerateCondition(this);
}
