using Sirenix.OdinInspector;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour
{
    [SerializeField] private float gravity = -13.0f;


    [SerializeField] protected Animator animator;
    [SerializeField] protected CharacterController controller;
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected FieldOfView view;
    [Space]
    [SerializeField] protected ColliderTrigger headCollider;
    [SerializeField] protected List<ColliderTrigger> colliders = new List<ColliderTrigger>();

    private Transform trans;
    public Transform Transform 
    {
        get
        {
            if (trans == null)
                trans = transform;
            return trans;
        }
    }

    protected bool isAlive = true;
    public bool IsAlive => isAlive;

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

    #region Private
    protected List<Transform> viewsTargers;

    protected int idleHash = -1;
    protected int idleIndexHash = -1;

    protected int velocityHash = -1;
    protected int directionHash = -1;

    protected int idleIndex = 0;
    protected int hitIndex = 1;
    protected int deathIndex = 1;

    protected float waypointTimer;
    protected float waitTime;

    protected Vector3 currentDestination;

    private Coroutine brainCoroutine = null;
    public bool IsBrainProccess => brainCoroutine != null;
    #endregion


    private void Awake()
    {
        //navmesh
        agent.updatePosition = false;
        //animations
        idleHash = Animator.StringToHash("IsIdle");
        idleIndexHash = Animator.StringToHash("IdleIndex");

        velocityHash = Animator.StringToHash("Velocity");
        directionHash = Animator.StringToHash("Direction");
        //view
        viewsTargers = view.visibleTargets;
        ////colliders
        //headCollider.onTriggerEnter +=;
        //for (int i = 0; i < colliders.Count; i++)
        //{
        //    colliders[i].onTriggerEnter +=;
        //}


        StartBrain();
    }
    Vector3 rootMotion;
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
        while (isAlive)
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

            view.StopView();
        }
    }

    protected virtual void Behavior() { }


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

    public enum AIState
    {
        Idle,
        Walk,
        Run,
    }
}