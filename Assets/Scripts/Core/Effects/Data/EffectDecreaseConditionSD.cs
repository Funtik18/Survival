using UnityEngine;

[CreateAssetMenu(menuName = "Game/Effects/Decrease Condition", fileName = "Effect")]
public class EffectDecreaseConditionSD : EffectSD 
{
    public float posionAmount = -10f;

    public override Effect GetEffect() => new EffectDecreaseCondition(this);
}