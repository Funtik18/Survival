using UnityEngine;

using Sirenix.OdinInspector;

public class Player : MonoBehaviour
{
	[SerializeField] private PlayerData data;
	public PlayerStats stats;


	public CharacterController characterController;
	[SerializeField] private PlayerController playerController;
	[SerializeField] private PlayerCamera playerCamera;
	public PlayerUI playerUI;

	[Space]
	[SerializeField] private bool isLockCursor = true;
	[SerializeField] private bool isMobileControll = false;

	private void Awake()
	{
		stats = new PlayerStats(data.statsData);

		playerUI.Setup(this);

		CheckCursor();

		playerController.Setup(this, playerCamera.transform, isMobileControll);
	}


	private void Update()
	{
		playerController.UpdateMovement();
	}
	private void LateUpdate()
	{
		playerController.UpdateMouseLook();
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
