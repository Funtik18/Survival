using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectDecreaseCondition : EffectBase<EffectDecreaseConditionSD>
{
    public EffectDecreaseCondition(EffectDecreaseConditionSD data) : base(data) { }

    protected override IEnumerator Execution()
	{
        onStart?.Invoke(this);
        player.Status.RestoreCondition(Data.posionAmount);
        onUpdate?.Invoke(this);
        yield return null;
        onEnd?.Invoke(this);
    }
}