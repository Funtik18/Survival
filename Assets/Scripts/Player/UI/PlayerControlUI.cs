using Sirenix.OdinInspector;

using UnityEngine;

public class PlayerControlUI : WindowUI
{
	[Title("Control")]
	public Joystick joystickMove;
	public FixedTouchField fieldLook;
	public RadialMenu radialMenu;

	[Title("Buttons")]
	public InteractionButton buttonInteraction;
	public CustomPointer buttonSpeedUp;

	[Title("Other")]
	public TargetPoint targetPoint;
	public GameObject blockPanel;
	public GameObject breakPanel;


	public void Setup(PlayerController playerController)
    {
		buttonSpeedUp.pointer.AddPressListener(playerController.SpeedUp);
		buttonSpeedUp.pointer.AddUnPressListener(playerController.SpeedDown);
	}

	#region Lock
	public void LockControl()
	{
		LockLook(false);
		LockMove(false);
	}
	public void UnLockControl()
	{
		LockLook(true);
		LockMove(true);
	}

	public void LockLook(bool trigger)
	{
		fieldLook.IsEnable = trigger; 
	}
	public void LockMove(bool trigger)
	{
		joystickMove.IsEnable = trigger;
		joystickMove.ResetHandleInput();
	}
	#endregion
}