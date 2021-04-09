using UnityEngine;

using Sirenix.OdinInspector;
using UnityEngine.Events;

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

	public PlayerStates states;
	public PlayerInventory Inventory;
	public Build Build;

	public PlayerController playerController;
	public PlayerCamera playerCamera;
	public PlayerUI playerUI;

	public ItemInspector itemInspector;

	[Space]
	[SerializeField] private bool isLockCursor = true;

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
		stats = new PlayerStats(data.statsData);
		states.AddAction(SwapStateChanged);

		Inventory.Init();
		Build.Init(this);

		playerController.Setup(stats, states);

		GeneralTime.Instance.AddAction(UpdateStats);

		playerUI.Setup(this);

		CheckCursor();
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

    #region Stats
    private UnityAction onStats; 
	private void SwapStateChanged(PlayerState state)
    {
		onStats = null;

		switch (state)
        {
			case PlayerState.Sleeping:
			{
				onStats += SleepingFormule;
			}
			break;
			case PlayerState.Standing:
			{
				onStats += StandingFormule;
			}
			break;
			case PlayerState.Walking:
			{
				onStats += WalkingFormule;
			}
			break;
			case PlayerState.Sprinting:
			{
				onStats += SprintingFormule;
			}
			break;
		}
	}
	private void UpdateStats()
	{
		onStats?.Invoke();

		ConditionFormule();
	}

	private void ConditionFormule()
    {
		if (stats.Warmth.CurrentValue == 0)
		{
			stats.Condition.CurrentValue -= (stats.Condition.Value * 4.5f) / 86400f;//-450.0%/d or ~18.75%/h
		}
		if (stats.Fatigue.CurrentValue == 0)
		{
			stats.Condition.CurrentValue -= (stats.Condition.Value * 0.25f) / 86400f;//-25.0%/d or ~1.04%/h
		}
		if (stats.Hungred.CurrentValue == 0)
        {
			stats.Condition.CurrentValue -= (stats.Condition.Value * 0.25f) / 86400f;//-25.0%/d or ~1.04%/h
		}
		if (stats.Thirst.CurrentValue == 0)
		{
			stats.Condition.CurrentValue -= (stats.Condition.Value * 0.5f) / 86400f;//-50.0%/d or ~2.08%/h
		}
	}

	private void AwakeFormule()
	{
		stats.Thirst.CurrentValue -= stats.Thirst.Value / (8f * 3600f);//100% / 8h or 12.5%/h
	}
	private void SleepingFormule()
	{
		stats.Hungred.CurrentValue -= 75f / 3600f;//75 cal/h
		stats.Thirst.CurrentValue -= stats.Thirst.Value / (12f * 3600f);//100% / 12h or ~8.33%/h
	}
	private void StandingFormule()
    {
		stats.Hungred.CurrentValue -= 125f / 3600f;//125 cal/h

		AwakeFormule();
	}
	private void WalkingFormule()
	{
		stats.Hungred.CurrentValue -= 200f / 3600f;//200 cal/h

		AwakeFormule();
	}
	private void SprintingFormule()
	{
		stats.Hungred.CurrentValue -= 400f / 3600f;//400 cal/h

		AwakeFormule();
	}
	#endregion

	#region Lock
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