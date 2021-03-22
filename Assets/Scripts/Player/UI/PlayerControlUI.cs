using System;
using System.Collections;
using System.Collections.Generic;

using Sirenix.OdinInspector;

using UnityEngine;

public class PlayerControlUI : MonoBehaviour
{
	[Title("Control")]
	public TargetPoint targetPoint;
	public Joystick joystickMove;
	public FixedTouchField touchField;

	[Title("Buttons")]
	public FixedTouchButton buttonInventory;
	public FixedTouchButton buttonPickUp;
	public FixedTouchButton buttonSpeedUp;

	public bool IsLookEnable { get; private set; }
	public bool IsMoveEnable { get; private set; }

	private void Awake()
	{
		UnBlockControl();
	}

	public void BlockControl()
	{
		LockLook(false);
		LockMove(false);
	}
	public void UnBlockControl()
	{
		LockLook(true);
		LockMove(true);
	}

	public void LockLook(bool trigger)
	{
		IsLookEnable = trigger;
		touchField.IsEnable = IsLookEnable; 
	}
	public void LockMove(bool trigger)
	{
		IsMoveEnable = trigger;
		joystickMove.IsEnable = IsMoveEnable;
	}
}