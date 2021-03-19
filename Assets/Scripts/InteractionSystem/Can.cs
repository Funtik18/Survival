using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Can : PickableItem
{
	public override void Interact()
	{
		Debug.LogError("Can");
	}
	public override void PickUp()
	{
		Destroy(gameObject);
	}
}