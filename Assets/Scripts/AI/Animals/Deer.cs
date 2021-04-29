using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deer : AI
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
    protected void BehaviorWander()
    {
        if (agent.remainingDistance >= agent.stoppingDistance && !agent.pathPending)
        {
            CurrentState = AIState.Walk;

            MoveAIRootMotion();
        }
        else
        {
            if (waypointTimer == 0)
            {
                CurrentState = AIState.Idle;
            }

            waypointTimer += Time.deltaTime;

            if (waypointTimer >= waitTime)
            {

                CurrentState = AIState.Walk;

                GenerateDestination(Transform.position, wanderInnerRadius, wanderOuterRadius);

                idleIndex = Random.Range(1, totalIdleAnimations + 1);
                animator.SetInteger(idleIndexHash, idleIndex);

                waitTime = Random.Range(3, 7);
                waypointTimer = 0;
            }
        }
    }
}
