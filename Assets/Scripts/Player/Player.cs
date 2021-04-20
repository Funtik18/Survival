using UnityEngine;

using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    #region Properties
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

	[SerializeField] private PlayerStatus status;
	public PlayerStatus Status => status;

	[SerializeField] private Inventory inventory;
	public Inventory Inventory => inventory;

	[SerializeField] private Build build;
	public Build Build => build;

	[SerializeField] private PlayerController controller;
	public PlayerController Controller => controller;

	[SerializeField] private PlayerCamera camera;
	public PlayerCamera Camera => camera;

	[SerializeField] private PlayerUI ui;
	public PlayerUI UI => ui;
	#endregion

	public float radius;

	public ItemInspector itemInspector;//make it

	[Space]
	[SerializeField] private bool isLockCursor = true;

	private bool isMoveLocked = false;
	private bool isLookLocked = false;

	private void Awake()
	{
		Status.Init(this);

		Controller.Init(this);
		
		Inventory.Init();
		UI.Setup(this);
		Build.Init(this);

		CheckCursor();
	}

	private void Update()
	{
		if (GeneralSettings.IsPlatformMobile)
		{
			Debug.LogError("Mobile");
			if (isMoveLocked == false)
			{
				Controller.UpdateMobileMovement();
			}
            if (isLookLocked == false)
            {
				Controller.UpdateMobileLook();
			}
		}
		else if (GeneralSettings.IsPlatformPC)
		{
			Debug.LogError("PC");
			if (isMoveLocked == false)
			{
				Controller.UpdatePCMovement();
			}
            if (isLookLocked == false)
            {
				Controller.UpdatePCLook();
			}
		}
	}


	public void ChangePosition(Vector3 position, Quaternion rotation)
    {
		StartCoroutine(SetPosition(position, rotation));
	}

	private IEnumerator SetPosition(Vector3 position, Quaternion rotation)
    {
		Lock();

		transform.position = position;
		transform.rotation = rotation;
		yield return null;
		
		UnLock();
	}

	#region Lock
	public void Lock()
    {
		isMoveLocked = true;
		isLookLocked = true;
        camera.LockVision();
        UI.controlUI.LockControl();
    }
	public void UnLock()
    {
		isMoveLocked = false;
		isLookLocked = false;
        camera.UnLockVision();
        UI.controlUI.UnLockControl();
    }

	public void LockMovement()
    {
		isMoveLocked = true;
		isLookLocked = true;
		UI.controlUI.LockControl();
	}
	public void UnLockMovement()
    {
		isMoveLocked = false;
		isLookLocked = false;
		UI.controlUI.UnLockControl();
	}
    #endregion

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

    [Button]
	private void Save()
	{
		SaveLoadManager.SavePlayerStatistics(this);
	}
}