using UnityEngine;

public class Player : MonoBehaviour
{
	public CharacterController characterController;
	[SerializeField] private PlayerController playerController;
	[SerializeField] private PlayerCamera playerCamera;
	public PlayerUI playerUI;

	[Space]
	[SerializeField] private bool isLockCursor = true;
	[SerializeField] private bool isMobileControll = false;

	private void Awake()
	{
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
