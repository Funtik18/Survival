using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectRegenerateCondition : EffectBase<EffectRegenerateConditionSD>
{
	private float actionCurrentStep = 0f;
	private Effect.EffectAction duration;

	public EffectRegenerateCondition(EffectRegenerateConditionSD data) : base(data) 
	{
		duration = data.duration;
	}

    protected override IEnumerator Execution()
    {
		onStart?.Invoke(this);
		while (actionCurrentStep < duration.actionAmount)
		{
			player.Status.RestoreCondition(Data.healAmount);
			onUpdate?.Invoke(this);
			yield return new WaitForSeconds(duration.actionDelay);
			actionCurrentStep += duration.actionStep;
		}
		onEnd?.Invoke(this);
	}
}
