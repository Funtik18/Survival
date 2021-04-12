using UnityEngine;

using Sirenix.OdinInspector;
using UnityEngine.Events;
using System.Collections;

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

	[SerializeField] private PlayerData Data;
	public PlayerStats Stats;

	public PlayerOpportunities Opportunities;
	public PlayerStates States;
	public PlayerInventory Inventory;
	public Build Build;

	public PlayerController Controller;
	public PlayerCamera playerCamera;
	public PlayerUI UI;

	public ItemInspector itemInspector;//make it

	[Space]
	[SerializeField] private bool isLockCursor = true;

	private bool isMoveLocked = false;
	private bool isLookLocked = false;

	private void Awake()
	{
		Stats = new PlayerStats(Data.statsData);
		States.AddAction(SwapStateChanged);

		Controller.Setup(Stats, States);

		Inventory.Init();
		Build.Init(this);

		GeneralTime.Instance.AddAction(UpdateStats);

		UI.Setup(this);

		Opportunities.Setup(this);

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
		if (Stats.Warmth.CurrentValue == 0)
		{
			Stats.Condition.CurrentValue -= (Stats.Condition.Value * 4.5f) / 86400f;//-450.0%/d or ~18.75%/h
		}
		if (Stats.Fatigue.CurrentValue == 0)
		{
			Stats.Condition.CurrentValue -= (Stats.Condition.Value * 0.25f) / 86400f;//-25.0%/d or ~1.04%/h
		}
		if (Stats.Hungred.CurrentValue == 0)
        {
			Stats.Condition.CurrentValue -= (Stats.Condition.Value * 0.25f) / 86400f;//-25.0%/d or ~1.04%/h
		}
		if (Stats.Thirst.CurrentValue == 0)
		{
			Stats.Condition.CurrentValue -= (Stats.Condition.Value * 0.5f) / 86400f;//-50.0%/d or ~2.08%/h
		}
	}

	private void AwakeFormule()
	{
		Stats.Thirst.CurrentValue -= Stats.Thirst.Value / (8f * 3600f);//100% / 8h or 12.5%/h
	}
	private void SleepingFormule()
	{
		Stats.Hungred.CurrentValue -= 75f / 3600f;//75 cal/h
		Stats.Thirst.CurrentValue -= Stats.Thirst.Value / (12f * 3600f);//100% / 12h or ~8.33%/h
	}
	private void StandingFormule()
    {
		Stats.Hungred.CurrentValue -= 125f / 3600f;//125 cal/h

		AwakeFormule();
	}
	private void WalkingFormule()
	{
		Stats.Hungred.CurrentValue -= 200f / 3600f;//200 cal/h

		AwakeFormule();
	}
	private void SprintingFormule()
	{
		Stats.Hungred.CurrentValue -= 400f / 3600f;//400 cal/h

		AwakeFormule();
	}
	#endregion

	#region Lock
	public void Lock()
    {
		isMoveLocked = true;
		isLookLocked = true;
        playerCamera.LockVision();
        UI.controlUI.LockControl();
    }
	public void UnLock()
    {
		isMoveLocked = false;
		isLookLocked = false;
        playerCamera.UnLockVision();
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

	private float Normalize(float value, float min, float max)
	{
		float normalized = (value - min) / (max - min);
		return normalized;
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

    [Button]
	private void Save()
	{
		SaveLoadManager.SavePlayerStatistics(this);
	}
}