using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class ColliderTrigger : MonoBehaviour
{
    public UnityAction<Transform> onTriggerEnter;

    public LayerMask targetMask;

    private void OnCollisionEnter(Collision collision)
    {
        if(((1 << collision.gameObject.layer) & targetMask) != 0)
        {
            onTriggerEnter?.Invoke(collision.transform);
        }
    }
}