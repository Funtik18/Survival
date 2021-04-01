using UnityEngine;

public interface IObservable
{
	bool IsObservable { get; }

	void StartObserve();
	void Observe();
	void EndObserve();
}
public interface IInteractable 
{
	bool IsInteractable { get; }

	void Interact();

}
public interface IEnterable
{

}
public interface IPullable
{

}

public interface IPerceptible : IObservable, IInteractable { }


[RequireComponent(typeof(Collider))]
public abstract class Object : MonoBehaviour
{
	[SerializeField] private Collider coll;
	public Collider Collider
	{
		get
		{
			if(coll == null)
				coll = GetComponent<Collider>();
			return coll;
		}
	}

	public void ColliderEnable(bool trigger)
	{
		Collider.enabled = trigger;
	}
}

public abstract class WorldBoard : Object, IObservable
{
	public virtual bool IsObservable => Collider.enabled;

	public virtual void StartObserve()
	{
		GeneralAvailability.TargetPoint.ShowPoint();
	}
	public virtual void Observe() { }
	public virtual void EndObserve() { }
}

public abstract class WorldObject : Object, IPerceptible
{
	public virtual bool IsObservable => Collider.enabled;
	public virtual bool IsInteractable => Collider.enabled;

	private InteractionButton button;
	protected InteractionButton Button
	{
		get
		{
			if (button == null)
			{
				button = GeneralAvailability.ButtonInteraction;
			}
			return button;
		}
	}

	public virtual void StartObserve()
	{
		GeneralAvailability.TargetPoint.ShowToolTip();
	}
	public virtual void Observe() { }
	public virtual void EndObserve() 
	{
		GeneralAvailability.TargetPoint.HideToolTip();
	}

	public virtual void Interact() { }
}