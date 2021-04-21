using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCSearch : MonoBehaviour
{
    [SerializeField] protected Animator animator;
    [SerializeField] private NavMeshAgent navMeshAgent;

    [SerializeField] private AIStates states;
    [SerializeField] private StatesEvents events;

    [Space]
    [SerializeField] private float wanderZone = 10f;
    private Vector3 origin;

    private Transform trans;
    public Transform Transform 
    {
        get
        {
            if(trans == null)
            {
                trans = transform;
            }
            return trans;
        }
    }


    private INPCState currentState;
    private StateWander wander;
    private StateIdle idle;

    private float thresholdDestination = 0.25f;

    private float howLongState;
    private float time = 0;

    private void Awake()
    {
        animator.applyRootMotion = false;

        origin = transform.position;
        wander = new StateWander(this, origin, wanderZone);
        idle = new StateIdle();
        idle.onIdle += IdleComplex;

        states.Init();

        ChangeState();

        //if (navMeshAgent)
        //{
        //    useNavMesh = true;
        //    navMeshAgent.stoppingDistance = contingencyDistance;
        //}

        //if (matchSurfaceRotation && transform.childCount > 0)
        //{
        //    transform.GetChild(0).gameObject.AddComponent<Common_SurfaceRotation>().SetRotationSpeed(surfaceRotationSpeed);
        //}
    }

    public IEnumerator Proccess()
    {
        yield return currentState.DoState();

        time += Time.deltaTime;

        if (time >= howLongState)
        {

            time = 0;
            ChangeState();

        }
    }
    public void Stop()
    {
        currentState.StopState();
    }

    private int lastRnd;
    public void ChangeState()
    {
        int rnd = Random.Range(0, 2);

        if (rnd != lastRnd)
        {
            Animation(states.walkingState, false);
        }

        lastRnd = rnd;

        if (currentState != null)
            currentState.StopState();

        if (rnd == 0)
        {
            Animation(states.walkingState, true);

            howLongState = Random.Range(5f, 10f);
            currentState = wander;
        }
        else
        {
            howLongState = Random.Range(1f, 10f);
            currentState = idle;
        }
    }

    private void Animation(NPCState state, bool trigger)
    {
        if (!string.IsNullOrEmpty(state.animationBool))
        {
            animator.SetBool(state.animationBool, trigger);
            
            if(state is NPCMovementState movementState)
            {
                navMeshAgent.speed = movementState.moveSpeed;
                navMeshAgent.angularSpeed = movementState.turnSpeed;
            }
        }
    }
    private void Animation(NPCState state)
    {
        if (!string.IsNullOrEmpty(state.animationBool))
        {
            animator.SetTrigger(state.animationBool);
        }
    }

    private void IdleComplex()
    {
        Animation(states.idleStates[0]);
    }

    public bool IsDestinationNear()
    {
        if ((Transform.position - navMeshAgent.destination).magnitude <= thresholdDestination)
            return true;
        return false;
    }
    public void SetDestination(Vector3 position)
    {
        navMeshAgent.SetDestination(position);
    }
    public void StopAgent()
    {
        navMeshAgent.SetDestination(Transform.position);
    }


    private void OnDrawGizmosSelected()
    {
        if (currentState != null)
            currentState.DrawGizmos();


        Gizmos.color = Color.red;
        Vector3[] corners = navMeshAgent.path.corners;

        for (int i = 0; i < corners.Length-1; i++)
        {
            Gizmos.DrawLine(corners[i], corners[i + 1]);
        }

        Gizmos.DrawWireSphere(origin == Vector3.zero ? transform.position : origin, wanderZone);
    }

    [System.Serializable]
    public class AIStates
    {
        public List<NPCIdleState> idleStates = new List<NPCIdleState>();

        public NPCMovementState walkingState;
        public NPCMovementState runningState;

        public List<NPCState> attackingStates = new List<NPCState>();
        public List<NPCState> deathStates = new List<NPCState>();

        [HideInInspector] public int totalIdleStateWeight;

        public void Init()
        {
            for (int i = 0; i < idleStates.Count; i++)
            {
                totalIdleStateWeight += idleStates[i].stateWeight;
            }
        }
    }

    [System.Serializable]
    public class StatesEvents
    {
        public UnityEvent deathEvent;
        public UnityEvent attackingEvent;
        public UnityEvent idleEvent;
        public UnityEvent movementEvent;
    }
}