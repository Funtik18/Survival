using UnityEngine;

using Sirenix.OdinInspector;
using System.Collections.Generic;

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

	[SerializeField] private PlayerInventory playerInventory;
	[SerializeField] private PlayerController playerController;
	public PlayerCamera playerCamera;
	public PlayerUI playerUI;

	[Space]
	[SerializeField] private bool isLockCursor = true;
	[SerializeField] private bool isMobileControll = false;


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


	private bool isMoveLocked = false;
	private bool isLookLocked = false;


	private void Awake()
	{
		//stats = new PlayerStats(data.statsData);

		playerUI.Setup(this);

		CheckCursor();

		playerController.Setup();
	}

	private void Update()
	{
		if (GeneralSettings.IsPlatformMobile)
		{
			Debug.LogError("Mobile");
			if (isMoveLocked == false)
			{
				playerController.UpdateMobileMovement();
			}
            if (isLookLocked == false)
            {
				playerController.UpdateMobileLook();
			}
		}
		else if (GeneralSettings.IsPlatformPC)
		{
			Debug.LogError("PC");
			if (isMoveLocked == false)
			{
				playerController.UpdatePCMovement();
			}
            if (isLookLocked == false)
            {
				playerController.UpdatePCLook();
			}
		}
	}
	
	public void AddItem(ItemScriptableData item)
	{
		playerInventory.AddItem(item);
	}

	public void Lock()
    {
		isMoveLocked = true;
		isLookLocked = true;
        playerCamera.LockVision();
        playerUI.controlUI.LockControl();
    }
	public void UnLock()
    {
		isMoveLocked = false;
		isLookLocked = false;
        playerCamera.UnLockVision();
        playerUI.controlUI.UnLockControl();
    }


	public void LockMovement()
    {
		isMoveLocked = true;
		isLookLocked = true;
		playerUI.controlUI.LockControl();
	}
	public void UnLockMovement()
    {
		isMoveLocked = false;
		isLookLocked = false;
		playerUI.controlUI.UnLockControl();
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
	//[TabGroup("PlayerStats")]
	//[HideLabel]
	//public PlayerStatsData statsData;

	[Button]
	private void Save()
	{
		SaveLoadManager.SavePlayerStatistics(this);
	}
}
