using System.Collections;

using UnityEngine;

public class Rabbit : Animal
{
    protected override void Behavior()
    {
        if (viewsTargers.Count > 0)
        {
            BehaviorRunAway();
        }
        else
        {
            BehaviorWander();
        }
    }

    protected override IEnumerator Death()
    {
        deathIndex = Random.Range(0, 6);

        return base.Death();
    }

    public override void Hit(HitZone.Zone zone)
    {
        if (zone == HitZone.Zone.Head)
        {
            Status.Condition.CurrentValue = 0;
        }
        else if (zone == HitZone.Zone.Chest)
        {
            Status.Condition.CurrentValue = 0;
        }
    }
}