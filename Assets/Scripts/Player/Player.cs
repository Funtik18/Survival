using UnityEngine;

using Sirenix.OdinInspector;
using System;

public class Player : MonoBehaviour
{
	private static Player instance;
	public static Player Instance
	{
		get
		{
			if(instance == null)
			{
				instance = FindObjectOfType<Player>();
			}
			return instance;
		}
	}

	[SerializeField] private PlayerData data;
	public PlayerStats stats;

	[SerializeField] private ItemInspector itemInspector;
	[SerializeField] private PlayerController playerController;
	public PlayerCamera playerCamera;
	public PlayerUI playerUI;

	[Space]
	[SerializeField] private bool isLockCursor = true;
	[SerializeField] private bool isMobileControll = false;

	private bool lockLook = false;
	private bool lockMovement = false;



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

	private void Awake()
	{
		stats = new PlayerStats(data.statsData);

		playerUI.Setup(this);

		CheckCursor();

		playerController.Setup(this, isMobileControll);
	}


	private void Update()
	{
		if(!lockMovement)
			playerController.UpdateMovement();
	}
	private void LateUpdate()
	{
		if(!lockLook)
			playerController.UpdateLook();
	}

	public void Lock(bool trigger)
	{
		LockMovement(trigger);
		LockLook(trigger);
	}
	public void LockMovement(bool trigger)
	{
		lockMovement = trigger;
		playerUI.joystickMove.IsEnable = trigger;
	}
	public void LockLook(bool trigger)
	{
		lockLook = trigger;
		playerUI.joystickMove.IsEnable = trigger;
	}

	private void CheckCursor()
	{
		if(isLockCursor)
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
	}
}
[System.Serializable]
public class PlayerData
{
	[TabGroup("PlayerStats")]
	[HideLabel]
	public PlayerStatsData statsData;
}
