using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class PickableItem : MonoBehaviour
{
	private Collider coll;
	public Collider Collider
	{
		get
		{
			if(coll == null)
				coll = GetComponent<Collider>();
			return coll;
		}
	}

	public bool IsPickable => Collider.enabled; 

	public void ColliderEnable(bool trigger)
	{
		Collider.enabled = trigger;
	}
}