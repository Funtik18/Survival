using Sirenix.OdinInspector;

using UnityEngine;

public class PlayerControlUI : MonoBehaviour
{
	public EnduranceUI endurance;

	[Title("Control")]
	public TargetPoint targetPoint;
	public Joystick joystickMove;
	public FixedTouchField fieldLook;
	public RadialMenu radialMenu;

	[Title("Buttons")]
	public InteractionButton buttonInteraction;
	public CustomButton buttonSpeedUp;

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