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

	[SerializeField] private Transform currentTransform;
	[SerializeField] private Camera playerCamera;
	[SerializeField] private CameraShaker shaker;
	[SerializeField] private float fieldOfView = 60f;
	[Space]
	[SerializeField] private LayerMask interactLayers;
	[SerializeField] private LayerMask ignoringLayers;
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
	private WaitForFixedUpdate visionSeconds = new WaitForFixedUpdate();

	private Coroutine fovCoroutine = null;
	private bool IsFovProccess => fovCoroutine != null;
	#endregion

	#region Properties
	public Transform Transform => currentTransform;

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

    #region FOV
    public void SetFieldOfView(float value, float t)
    {
        if (!IsFovProccess)
        {
			fovCoroutine = StartCoroutine(FOV(value, t));
        }
	}
	public void ResetFieldOfView(float t)
	{
		if (!IsFovProccess)
		{
			fovCoroutine = StartCoroutine(FOV(fieldOfView, t));
        }
        else
        {
			StopFOV();
			fovCoroutine = StartCoroutine(FOV(fieldOfView, t));
		}
	}
	private IEnumerator FOV(float fov, float t)
    {
		float time = 0;
		float startFOV = playerCamera.fieldOfView;
		float endFov = fov;
		while(time < t)
        {
			playerCamera.fieldOfView = Mathf.Lerp(startFOV, endFov, time / t);

			time += Time.deltaTime;
			yield return null;
		}
		playerCamera.fieldOfView = endFov;
		StopFOV();
	}
	private void StopFOV()
    {
		if (IsFovProccess)
		{
			StopCoroutine(fovCoroutine);
			fovCoroutine = null;
		}
	}
    #endregion

    #region Vision
	private IObservable currentObserv;
	private IObservable CurrentObserv
    {
		get => currentObserv;

		set
        {
			if (currentObserv != value)
            {
				currentObserv?.EndObserve();
				currentObserv = value;
				currentObserv?.StartObserve();
            }
            else
            {
				currentObserv?.Observe();
			}
		}
	}

	private void Dispose()
	{
		if (CurrentObserv != null)
		{
			CurrentObserv = null;
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
				Ray ray = new Ray(currentTransform.position, currentTransform.forward);

                //каст для мира
                if (Physics.Raycast(ray, out hit, maxRayDistance, ~ignoringLayers))
                {
                    lastHitPoint = hit.point;

					//переделать
                    collidersIntersects.Clear();
                    collidersIntersects.AddRange(Physics.OverlapSphere(hit.point, sphereRadius, interactLayers));

                    if (collidersIntersects.Count > 0)
                        GeneralAvailability.TargetPoint.ShowPoint();
                    else
                        GeneralAvailability.TargetPoint.HidePoint();

                    //каст для интерактивных объектов
                    if (Physics.Raycast(ray, out hit, rayDistance, interactLayers))
					{
						if(hit.transform.parent == null)
							CurrentObserv = hit.transform.GetComponent<IObservable>();
						else
							CurrentObserv = hit.transform.GetComponentInParent<IObservable>();
					}
					else
					{
						Dispose();
					}
                }
                else
                {
					Dispose();
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

		Dispose();
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