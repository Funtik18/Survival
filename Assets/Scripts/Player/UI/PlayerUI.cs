using System;

using UnityEngine;

public class PlayerUI : MonoBehaviour
{
	//states
	public EnduranceUI endurance;

	//control
	public Joystick joystickMove;
	public FixedTouchField touchField;

	//buttons
	public FixedTouchButton buttonSpeedUp;

	public void Setup(Player player)
	{
		endurance.Setup(player.stats.Endurance);
	}
}
