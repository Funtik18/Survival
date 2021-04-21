using System.Collections;

using UnityEngine;

public class StateWander : INPCState
{
    private float wanderZone = 10f;
    private Vector3 origin;

    private Vector3 targetDestination;

    private NPCSearch search;
    private Transform searchTransform;

    public StateWander(NPCSearch search, Vector3 origin, float wanderZone)
    {
        this.search = search;
        searchTransform = search.Transform;
        this.origin = origin;
        this.wanderZone = wanderZone;
    }

    public IEnumerator DoState()
    {
        if (!search.IsDestinationNear())
        {
            Debug.LogError("Wander");
        }
        else
        {
            search.SetDestination(GetRandonPointInRange());
        }

        yield return null;
    }

    public void StopState()
    {
        search.StopAgent();
    }

    public void DrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(targetDestination, 0.2f);
    }

    private Vector3 GetRandonPointInRange()
    {
        Vector3 randomPoint = origin + Random.insideUnitSphere * wanderZone;
        targetDestination = new Vector3(randomPoint.x, searchTransform.position.y, randomPoint.z);
        return targetDestination;
    }
}