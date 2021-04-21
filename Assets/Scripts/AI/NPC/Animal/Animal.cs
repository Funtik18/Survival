using Sirenix.OdinInspector;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.AI;
using System.Collections;



public class Animal : NPC
{
    [SerializeField] private AnimalSD sd;
    [SerializeField] private AnimalStatsData data;

    [Tooltip("How far this animal can sense a predator.")]
    [SerializeField] private float awareness = 30f;

    [Tooltip("How far this animal can sense it's prey.")]
    [SerializeField] private float scent = 30f;
    private float originalScent = 0f;

    [SerializeField] private GizmosDebug debug;


    protected override void Awake()
    {
        AnimalOverseer.AnimalsAll.Add(this);

        base.Awake();
    }

    private void OnDestroy()
    {
        AnimalOverseer.AnimalsAll.Remove(this);
    }

    private void OnDrawGizmosSelected()
    {
        if (!debug.showGizmos)
            return;

        if (debug.drawAwarenessRange)
        {
            //Draw circle radius for Awarness.
            Gizmos.color = debug.awarnessColor;
            Gizmos.DrawWireSphere(transform.position, awareness);
        }

        if (debug.drawScentRange)
        {
            //Draw circle radius for Scent.
            Gizmos.color = debug.scentColor;
            Gizmos.DrawWireSphere(transform.position, scent);
        }

        if (!Application.isPlaying)
            return;

        // Draw target position.
        //if (useNavMesh)
        //{
        //    if (navMeshAgent.remainingDistance > 1f)
        //    {
        //        Gizmos.DrawSphere(navMeshAgent.destination + new Vector3(0f, 0.1f, 0f), 0.2f);
        //        Gizmos.DrawLine(transform.position, navMeshAgent.destination);
        //    }
        //}
        //else
        //{
        //    if (targetLocation != Vector3.zero)
        //    {
        //        Gizmos.DrawSphere(targetLocation + new Vector3(0f, 0.1f, 0f), 0.2f);
        //        Gizmos.DrawLine(transform.position, targetLocation);
        //    }
        //}
    }

    [System.Serializable]
    public class GizmosDebug
    {
        public bool showGizmos;

        [ShowIf("showGizmos")]
        public bool drawWanderRange;
        [ShowIf("showGizmos")]
        public Color distanceColor;

        [ShowIf("showGizmos")]
        public bool drawAwarenessRange;
        [ShowIf("showGizmos")]
        public Color awarnessColor;

        [ShowIf("showGizmos")]
        public bool drawScentRange;
        [ShowIf("showGizmos")]
        public Color scentColor;
    }
}
[System.Serializable]
public class AnimalStatsData
{

}