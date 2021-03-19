using System.Collections;
using System.Collections.Generic;
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


	private Coroutine shakeCoroutine = null;
	private bool IsShakeProccess => shakeCoroutine != null;


	private void Update()//need hit in 0.25f sec
	{
		Debug.DrawLine(transform.position, transform.position + (transform.forward * 5), Color.blue);

		RaycastHit hit;

		if(Physics.Linecast(transform.position, transform.position + (transform.forward * 5), out hit))
		{
			PickableItem obj = hit.collider.gameObject.GetComponent<PickableItem>();
			if(obj != null)
			{
				obj.Interact();

				if(Input.GetKeyDown(KeyCode.Space))
				{
					obj.PickUp();
				}

				Debug.LogError(hit.collider.gameObject.name, hit.collider.gameObject);
			}
		}
	}



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

	//private void Update()
	//{
	//	if(Input.GetKeyDown(KeyCode.Space))
	//	{
	//		StartCoroutine(CameraShake());
	//	}
	//}


	//IEnumerator CameraShake()
	//{
	//	Vector3 originalPos = Transform.localPosition;
	//	float elapsed = 0f;

	//	while(elapsed < duration)
	//	{
	//		float x = Random.Range(-1f, 1f) * magnitude;
	//		float y = Random.Range(-1f, 1f) * magnitude;

	//		Transform.localPosition = new Vector3(x, y, originalPos.z);

	//		elapsed += Time.deltaTime;

	//		yield return null;
	//	}

	//	Transform.localPosition = originalPos;
	//}
}
