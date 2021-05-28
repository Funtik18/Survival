using Sirenix.OdinInspector;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI : WorldEntity
{
    [SerializeField] private Transform currentTransform;
    [Space]
    [SerializeField] private bool isBrain = true;
    [SerializeField] private AIData data;

    [SerializeField] private Collider interaction;
    [SerializeField] private List<HitZone> hitZones = new List<HitZone>();

    [SerializeField] private float gravity = -13.0f;

    [SerializeField] protected Animator animator;
    [SerializeField] protected CharacterController controller;
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected FieldOfView view;

    public Transform Transform => currentTransform;
    public AIStatus Status { get; private set; }

    private AIState currentState;
    public AIState CurrentState
    {
        get => currentState;
        set
        {
            currentState = value;

            animator.SetBool(idleHash, currentState == AIState.Idle);
        }
    }

    [Space]
    public float Acceleration = 1;
    public float Deceleration = 0.5f;

    public float WalkAnimationSpeed = 1;
    public float RunAnimationSpeed = 1;

    public int totalIdleAnimations = 0;

    #region Private
    private Coroutine brainCoroutine = null;
    public bool IsBrainProccess => brainCoroutine != null;

    protected List<Transform> viewsTargers;

    protected int idleHash = -1;
    protected int idleIndexHash = -1;
    protected int idleIndex = 0;

    protected int deadHash = -1;
    protected int deathIndexHash = -1;
    protected int deathIndex = 0;

    protected int velocityHash = -1;
    protected int directionHash = -1;

    protected int hitIndex = 1;

    private Vector3 rootMotion;
    #endregion


    protected virtual void Awake()
    {
        //navmesh
        agent.updatePosition = false;
        //animations
        idleHash = Animator.StringToHash("IsIdle");
        idleIndexHash = Animator.StringToHash("IdleIndex");

        deadHash = Animator.StringToHash("Dead");
        deathIndexHash = Animator.StringToHash("DeathIndex");

        velocityHash = Animator.StringToHash("Velocity");
        directionHash = Animator.StringToHash("Direction");

        //view
        viewsTargers = view.visibleTargets;


        Status = new AIStatus(data.status);
        Status.Condition.onCurrentValueZero += StopBrain;

        StartBrain();
    }
    private void Update()
    {
        if (controller)
        {
            if (!controller.isGrounded)
            {
                rootMotion.y += gravity * Time.deltaTime;
            }
            controller.Move(rootMotion);
            rootMotion = Vector3.zero;
        }
    }
    private void OnAnimatorMove()
    {
        agent.nextPosition = Transform.position;

        rootMotion += animator.deltaPosition;

        //Transform.position = animator.rootPosition;
        Transform.rotation = animator.rootRotation;
    }

    #region Brain
    private void StartBrain()
    {
        if (!IsBrainProccess)
        {
            view.StartView();

            CurrentState = AIState.Idle;

            brainCoroutine = StartCoroutine(Brain());
        }
    }
    private IEnumerator Brain()
    {
        while (isBrain)
        {
            Behavior();

            yield return null;
        }
        //death
        StopBrain();
    }
    private void StopBrain()
    {
        if (IsBrainProccess)
        {
            StopCoroutine(brainCoroutine);
            brainCoroutine = null;

            StartCoroutine(Death());
        }
    }
    #endregion

    protected virtual void Behavior() { }
    protected virtual IEnumerator Death() { yield return null; }
    public virtual void Hit(HitZone.Zone zone) { }

    protected void MoveAIRootMotion()
    {
        float speed = agent.desiredVelocity.magnitude;
        Vector3 direction = Quaternion.Inverse(Transform.rotation) * agent.desiredVelocity;
        float angle = Mathf.Atan2(direction.x, direction.z) * 180.0f / Mathf.PI;

        if (agent.remainingDistance >= agent.stoppingDistance && !agent.pathPending)
        {
            if (CurrentState == AIState.Run)
            {
                agent.speed = Mathf.Min(agent.speed + Acceleration * Time.deltaTime, RunAnimationSpeed);
            }
            else if (CurrentState == AIState.Walk)
            {
                if (WalkAnimationSpeed <= 1f)
                {
                    if(Mathf.Abs(angle) > 100)
                    {
                        agent.speed = 0;
                    }
                    agent.speed = Mathf.Max(agent.speed - 1 * Time.deltaTime, WalkAnimationSpeed * 0.5f);
                }
                else if (WalkAnimationSpeed > 1f)
                {
                    agent.speed = Mathf.Max(agent.speed - 1 * Time.deltaTime, 0.5f);
                }
            }
        }
        else if (agent.remainingDistance < agent.stoppingDistance && !agent.pathPending)
        {
            agent.speed = Mathf.Max(agent.speed - Deceleration * Time.deltaTime, 0f);
        }
        else
        {
            agent.speed = Mathf.Max(agent.speed - Deceleration * Time.deltaTime, 0f);
        }

        animator.SetFloat(directionHash, angle);
        animator.SetFloat(velocityHash, speed, 0.3f, Time.deltaTime);
    }


    protected bool CheckPath(NavMeshAgent agent, Vector3 destination)
    {
        NavMeshPath path = new NavMeshPath();
        agent.CalculatePath(destination, path);
        if (path.status == NavMeshPathStatus.PathComplete)
        {
            return true;
        }
        return false;
    }


    protected virtual void Diable()
    {
        animator.enabled = false;
        controller.enabled = false;
        agent.enabled = false;
        view.StopView();

        for (int i = 0; i < hitZones.Count; i++)
        {
            hitZones[i].Collider.enabled = false;
        }

        interaction.enabled = true;
    }


    public enum AIState
    {
        Idle,
        Walk,
        Run,
    }
}
public class AIStatus
{
    public StatCondition Condition;

    public bool IsAlive => !Condition.IsEquilZero;

    public AIStatus(AIStatusData data)
    {
        Condition = new StatCondition(data.condition);
    }
}
[System.Serializable]
public class AIData 
{
    [TabGroup("AIStats")]
    [HideLabel]
    public AIStatusData status;
}
[System.Serializable]
public struct AIStatusData
{
    public StatBarData condition;
}