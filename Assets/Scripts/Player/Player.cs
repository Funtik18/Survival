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

	[SerializeField] private ItemInspector itemInspector;
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

	private void Awake()
	{
		stats = new PlayerStats(data.statsData);

		playerUI.Setup(this);

		CheckCursor();

		playerController.Setup(Transform, playerCamera.Transform, playerUI.controlUI, isMobileControll);
	}


	private void Update()
	{
		if(playerUI.controlUI.IsMoveEnable)
			playerController.UpdateMovement();
	}
	private void LateUpdate()
	{
		if(playerUI.controlUI.IsLookEnable)
			playerController.UpdateLook();
	}

	public void AddItem(ItemModel item)
	{
		data.items.Add(item);
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
	public List<ItemModel> items = new List<ItemModel>();
	[TabGroup("PlayerStats")]
	[HideLabel]
	public PlayerStatsData statsData;

	[Button]
	private void Save()
	{
		SaveLoadManager.SavePlayerStatistics(this);
	}
}
