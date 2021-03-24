using System.Collections;
using UnityEngine;

using EZCameraShake;

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

	[SerializeField] private LayerMask castLayers;

	[Space]
	[SerializeField] private float rayDistance = 5f;

	//coroutines
	private Coroutine shakeCoroutine = null;
	private bool IsShakeProccess => shakeCoroutine != null;

	private Coroutine visionCoroutine = null;
	private bool IsVisionProccess => visionCoroutine != null;
	private WaitForSeconds visionSeconds = new WaitForSeconds(0.1f);
	private bool isVisionBlocked = false;

	//properties
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
			if(isVisionBlocked == false)
            {
				RaycastHit hit;
				Ray ray = new Ray(Transform.position, Transform.forward);

				if (Physics.Raycast(ray, out hit, rayDistance, castLayers))
					CurrentCollider = hit.collider;
				else
                    DisposeCollider();

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

	public void LockVision()
	{
		isVisionBlocked = true;

		DisposeCollider();
	}
	public void UnLockVision()
	{
		isVisionBlocked = false;
	}
}
//private void Update()
//{
//	if (isVisionBlocked == false)
//		if (Input.GetKey(KeyCode.E))//need cash
//		{
//			Debug.LogError("+++");
//			Interact();
//		}
//}

//private void Interact()
//{
//	if (currentCollider != null)
//	{
//		IInteractable interaction = currentCollider.GetComponent<IInteractable>();
//		if (interaction != null)
//		{
//			interaction.Interact();
//		}
//		else
//		{
//			Debug.LogError("Nelza");
//		}
//	}
//}