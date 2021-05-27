using Sirenix.OdinInspector;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : AI
{
    [Space]
    [SerializeField] private AnimalSD sdata;
    public AnimalSD SData => sdata;

    [SerializeField] private bool isBreakable;
    public bool IsBreakable => isBreakable;

    [Space]
    [Min(1), MaxValue("wanderOuterRadius")]
    public float wanderInnerRadius;
    [MinValue("wanderInnerRadius")]
    public float wanderOuterRadius;
    [Space]
    [Min(1), MaxValue("awayOuterRadius")]
    public float awayInnerRadius;
    [MinValue("awayInnerRadius")]
    public float awayOuterRadius;

    public ItemDataWrapper Meat { get; protected set; }


    protected Vector3 currentDestination;

    protected float waypointTimer;
    protected float waitTime;

    protected override void Awake()
    {
        base.Awake();

        Meat = SData.GenerateMeat();
    }

    protected override IEnumerator Death()
    {
        animator.SetInteger(deathIndexHash, deathIndex);
        animator.SetTrigger(deadHash);

        yield return new WaitForSeconds(3f);
        Diable();
    }

    public override void StartObserve()
    {
        if (!Status.IsAlive)
        {
            GeneralAvailability.TargetPoint.SetToolTipText(sdata.animalName).ShowToolTip();

            InteractionButton.pointer.AddPressListener(OpenHarestingWindow);
            InteractionButton.SetIconOnInteraction();
            InteractionButton.OpenButton();
        }
    }
    public override void EndObserve()
    {
        if (!Status.IsAlive)
        {
            GeneralAvailability.TargetPoint.HideToolTip();

            InteractionButton.CloseButton();
            InteractionButton.pointer.RemoveAllPressListeners();
        }
    }

    private void OpenHarestingWindow()
    {
        GeneralAvailability.PlayerUI.OpenHarvestingCarcass(this);
    }

    /// BEHAVIORS
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
    protected void BehaviorRunAway()
    {
        if (CurrentState != AIState.Run)
        {
            GenerateDestination(Transform.position, awayInnerRadius, awayOuterRadius);
        }

        if (agent.remainingDistance >= agent.stoppingDistance && !agent.pathPending)
        {
            CurrentState = AIState.Run;
            MoveAIRootMotion();
        }
        else
        {
            GenerateDestination(Transform.position, awayInnerRadius, awayOuterRadius);
        }
    }

    protected void GenerateDestination(Vector3 origin, float innerRadius, float outerRadius)
    {
        currentDestination = ExtensionRandom.RandomPointInAnnulus(origin, innerRadius, outerRadius);
        currentDestination.y = Terrain.activeTerrain.SampleHeight(currentDestination);
        if (CheckPath(agent, currentDestination))
        {
            agent.SetDestination(currentDestination);
        }
        else
        {
            GenerateDestination(origin, innerRadius, outerRadius);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(currentDestination, 0.25f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(currentDestination, agent.stoppingDistance);

        //path
        Gizmos.color = Color.yellow;
        Vector3[] corners = agent.path.corners;

        for (int i = 0; i < corners.Length - 1; i++)
        {
            Gizmos.DrawLine(corners[i], corners[i + 1]);
        }
    }
}
public class HarvestingCarcass
{

}

