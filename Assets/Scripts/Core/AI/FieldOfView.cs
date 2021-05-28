using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FieldOfView : MonoBehaviour
{
	public float viewRadius;
	[Range(0, 360)]
	public float viewAngle;

	[HideInInspector] public List<Transform> visibleTargets = new List<Transform>();
	[Space]
	[SerializeField] private LayerMask targetMask;
	[SerializeField] private LayerMask obstacleMask;

	[SerializeField] private float delay = 0.2f;

	private Transform trans;
	public Transform Transform
	{
		get
		{
			if (trans == null)
			{
				trans = transform;
			}
			return trans;
		}
	}

	private Coroutine viewCoroutine = null;
	public bool IsViewProccess => viewCoroutine != null;

	private WaitForSeconds seconds;

	public void StartView()
    {
        if (!IsViewProccess)
        {
			seconds = new WaitForSeconds(delay);
			viewCoroutine = StartCoroutine(FindTargetsWithDelay());
		}
	}
	private IEnumerator FindTargetsWithDelay()
	{
		while (true)
		{
			yield return seconds;
			FindVisibleTargets();
		}

		StopView();
	}
	public void StopView()
    {
        if (IsViewProccess)
        {
			StopCoroutine(viewCoroutine);
			viewCoroutine = null;
		}
	}

	private void FindVisibleTargets()
	{
		visibleTargets.Clear();
		Collider[] targetsInViewRadius = Physics.OverlapSphere(Transform.position, viewRadius, targetMask);

		for (int i = 0; i < targetsInViewRadius.Length; i++)
		{
			Transform target = targetsInViewRadius[i].transform;
			Vector3 dirToTarget = (target.position - Transform.position).normalized;
			if (Vector3.Angle(Transform.forward, dirToTarget) < viewAngle / 2)
			{
				float dstToTarget = Vector3.Distance(Transform.position, target.position);

				if (!Physics.Raycast(Transform.position, dirToTarget, dstToTarget, obstacleMask))
				{
					visibleTargets.Add(target);
				}
			}
		}
	}

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
	{
		if (!angleIsGlobal)
		{
			angleInDegrees += Transform.eulerAngles.y;
		}

		return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
	}
}