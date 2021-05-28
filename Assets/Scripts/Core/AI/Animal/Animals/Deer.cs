using System.Collections;

using UnityEngine;

public class Deer : Animal
{
    protected override void Behavior()
    {
        base.Behavior();

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
        deathIndex = Random.Range(3,5);

        return base.Death();
    }

    public override void Hit(HitZone.Zone zone)
    {
        if (zone == HitZone.Zone.Head)
        {
            Status.Condition.CurrentValue = 0;
        }
        else if(zone == HitZone.Zone.Neck)
        {
            Status.Condition.CurrentValue = 0;
        }
        else if (zone == HitZone.Zone.Chest)
        {
            Status.Condition.CurrentValue = 0;
        }
        else if(zone == HitZone.Zone.Hind)
        {
            Status.Condition.CurrentValue = 0;
        }
        else if(zone == HitZone.Zone.Foot)
        {
            Status.Condition.CurrentValue = 0;
        }
    }
}