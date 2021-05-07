using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratePlayerPosition : MonoBehaviour
{
    public bool randomPositionOnAwake = false;
    
    [SerializeField] private List<Transform> points = new List<Transform>();

    private void Awake()
    {
        if (randomPositionOnAwake)
        {
            Debug.LogError("Teleport");
            GeneralAvailability.Player.ChangePosition(points.GetRandomItem().position, Quaternion.identity);
        }
    }


    private void OnDrawGizmos()
    {
        for (int i = 0; i < points.Count; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(points[i].position, 1f);
            Gizmos.DrawWireSphere(points[i].position, 5f);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(points[i].position, points[i].position + points[i].forward * 5f);
        }
    }
}
