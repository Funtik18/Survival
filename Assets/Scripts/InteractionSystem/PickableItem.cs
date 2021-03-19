using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableItem : Interaction, IPickable
{
	public virtual void PickUp()
	{
		Debug.LogError("PickUp");
	}
}