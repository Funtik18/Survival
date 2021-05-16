using UnityEngine;

public class Wolf : AI
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

                if (Random.Range(0, 100) < 65)
                {
                    CurrentState = AIState.Walk;
                }
                else
                {
                    CurrentState = AIState.Run;
                }

                GenerateDestination(Transform.position, wanderInnerRadius, wanderOuterRadius);

                idleIndex = Random.Range(1, totalIdleAnimations + 1);
                animator.SetInteger(idleIndexHash, idleIndex);

                waitTime = Random.Range(3, 7);
                waypointTimer = 0;
            }
        }
    }
}