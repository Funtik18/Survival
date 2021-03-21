using Sirenix.OdinInspector;

using UnityEngine;

public class PlayerUI : MonoBehaviour
{
	//states
	public EnduranceUI endurance;

	[Title("Control")]
	public TargetPoint targetPoint;
	public Joystick joystickMove;
	public FixedTouchField touchField;

	[Title("Buttons")]
	public FixedTouchButton buttonPickUp;
	public FixedTouchButton buttonSpeedUp;

	[Title("Windows")]
	public WindowItemInspector itemInspector;

	public void Setup(Player player)
	{
		endurance.Setup(player.stats.Endurance);
	}
}
