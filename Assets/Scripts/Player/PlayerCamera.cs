using System.Collections.Generic;
using System.Collections;
using UnityEngine;

using EZCameraShake;
using System.Linq;

public class PlayerCamera : MonoBehaviour
{
	private static PlayerCamera instance;
	public static PlayerCamera Instance
	{
		get
		{
			if(instance == null)
			{
				instance = FindObjectOfType<PlayerCamera>();
			}
			return instance;
		}
	}

	[SerializeField] private Camera playerCamera;
	[SerializeField] private CameraShaker shaker;

	[SerializeField] private LayerMask interactLayers;
	[Space]
	[SerializeField] private float maxRayDistance = 5f;
	[SerializeField] private float rayDistance = 5f;
	[SerializeField] private float sphereRadius = 1f;

	[HideInInspector] public List<Collider> collidersIntersects = new List<Collider>();

	#region Coroutines
	private Coroutine shakeCoroutine = null;
	private bool IsShakeProccess => shakeCoroutine != null;

	private Coroutine visionCoroutine = null;
	public bool IsVisionProccess => visionCoroutine != null;
	private WaitForSeconds visionSeconds = new WaitForSeconds(0.1f);
    #endregion

    #region Properties
    private Transform trans;
	public Transform Transform
	{
		get
		{
			if(trans == null)
				trans = transform;
			return trans;
		}
	}
	public bool IsVisionBlocked { get; private set; }

	private CameraShakeInstance HandheldCamera
	{
		get
		{
			CameraShakeInstance c = new CameraShakeInstance(1f, 0.25f, 5f, 10f);
			c.DeleteOnInactive = true;
			c.PositionInfluence = Vector3.zero;
			c.RotationInfluence = new Vector3(2, 1f, 1f);
			return c;
		}
	}
	#endregion

	private Vector3 lastHitPoint;

    private void Awake()
	{
		StartVision();
	}

    #region Vision
    private Collider currentCollider = null;
	private Collider CurrentCollider
    {
		get => currentCollider;
        set
        {
			if(value != currentCollider || value == null)
            {
				if(currentCollider != null)
					currentCollider.GetComponent<IObservable>().EndObserve();

				currentCollider = value;
				
				if (currentCollider != null)
					currentCollider.GetComponent<IObservable>().StartObserve();
			}
		}
	}
	private void DisposeCollider()
	{
		if (CurrentCollider != null)
		{
			CurrentCollider = null;
		}
	}

	private void StartVision()
	{
		if(!IsVisionProccess)
		{
			visionCoroutine = StartCoroutine(Vision());
		}
	}
	private IEnumerator Vision()
	{
		while(true)
		{
			if (IsVisionBlocked == false)
            {
				RaycastHit hit;
				Ray ray = new Ray(Transform.position, Transform.forward);

                //каст для мира
                if (Physics.Raycast(ray, out hit, maxRayDistance))
                {
                    lastHitPoint = hit.point;

                    collidersIntersects.Clear();
                    collidersIntersects.AddRange(Physics.OverlapSphere(hit.point, sphereRadius, interactLayers));
                    if (collidersIntersects.Count > 0)
                        GeneralAvailability.TargetPoint.ShowPoint();
                    else
                        GeneralAvailability.TargetPoint.HidePoint();

                    //каст для интерактивных объектов
                    if (Physics.Raycast(ray, out hit, rayDistance, interactLayers))
					{
						CurrentCollider = hit.collider;
					}
					else
					{
						DisposeCollider();
					}
                }
                else
                {
                    GeneralAvailability.TargetPoint.HidePoint();
                }

                //Debug.DrawLine(Transform.position, Transform.position + (Transform.forward * rayDistance), Color.blue);

            }
			yield return visionSeconds;
		}
		StopVision();
	}
    private void StopVision()
	{
		if(IsVisionProccess)
		{
			StopCoroutine(visionCoroutine);
			visionCoroutine = null;
		}
	}

	public void LockVision()
	{
		IsVisionBlocked = true;

		GeneralAvailability.TargetPoint.HidePoint();

		DisposeCollider();
	}
	public void UnLockVision()
	{
		IsVisionBlocked = false;
	}
	#endregion

	#region Idle Shake
	public void StartIdleShake()
	{
		if(!IsShakeProccess)
		{
			shakeCoroutine = StartCoroutine(IdleShake());
		}
	}
	private IEnumerator IdleShake()
	{
		CameraShakeInstance s = HandheldCamera;
		shaker.Shake(s);
		while(s.CurrentState != CameraShakeState.Inactive)
		{
			yield return null;
		}

		StopIdleShake();
	}
	public void StopIdleShake()
	{
		if(IsShakeProccess)
		{
			StopCoroutine(shakeCoroutine);
			shakeCoroutine = null;
		}
	}
	#endregion


	private void OnDrawGizmos()
	{
		if (IsVisionBlocked == false)
		{
			Color sphereColor = Color.red;
			sphereColor.a = 0.1f;
			Gizmos.color = sphereColor;
			Gizmos.DrawSphere(lastHitPoint, sphereRadius);

			Gizmos.color = Color.red;
			for (int i = 0; i < collidersIntersects.Count; i++)
			{
				if(collidersIntersects[i] != null)
				Gizmos.DrawLine(lastHitPoint, collidersIntersects[i].transform.position);
			}
		}
	}
}