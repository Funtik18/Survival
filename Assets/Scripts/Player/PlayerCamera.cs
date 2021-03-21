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

	[SerializeField] private ItemInspector inspector;
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
	private WaitForSeconds visionSeconds = new WaitForSeconds(0.25f);

	//cash
	private PlayerUI playerUI;

	private Item itemCashed = null;
	private bool checkExit = false;

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
		playerUI = Player.Instance.playerUI;

		playerUI.buttonPickUp.onClicked += InspectorLook;
		
		StartVision();
	}

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.E))
		{
			InspectorLook();
		}
	}

	private void InspectorLook()
	{
		if(itemCashed && itemCashed.IsPickable)
		{
			inspector.SetItem(itemCashed);
		}
	}

	#region Vision
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
			Debug.DrawLine(Transform.position, Transform.position + (Transform.forward * rayDistance), Color.blue);

			RaycastHit hit;
			Ray ray = new Ray(Transform.position, Transform.forward);

			if(Physics.Raycast(ray, out hit, rayDistance, castLayers))
			{
				Item item = hit.collider.GetComponent<Item>();
				if(item != null && item != itemCashed)
				{
					itemCashed = item;

					playerUI.buttonPickUp.IsActive(true);

					playerUI.targetPoint.ShowPoint();
					playerUI.targetPoint.SetToolTipText(itemCashed.data.name).ShowToolTip();
					checkExit = true;
				}
			}
			else
			{
				if(checkExit)
				{
					playerUI.buttonPickUp.IsActive(false);

					playerUI.targetPoint.HidePoint();
					itemCashed = null;
					checkExit = false;
				}
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
}