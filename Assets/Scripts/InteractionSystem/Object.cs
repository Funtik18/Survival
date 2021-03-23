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

	private PlayerControlUI controlUI;
	protected PlayerControlUI ControlUI
	{
		get
		{
			if(controlUI == null)
			{
				controlUI = Player.Instance.playerUI.controlUI;
			}
			return controlUI;
		}
	}
}

public abstract class WorldBoard : Object, IObservable
{
	public virtual bool IsObservable => Collider.enabled;

	public virtual void StartObserve()
	{
		ControlUI.targetPoint.ShowPoint();
	}
	public virtual void Observe() { }
	public virtual void EndObserve()
	{
		ControlUI.targetPoint.HidePoint();
	}
}


public abstract class WorldObject : Object, IPerceptible
{
	public virtual bool IsObservable => Collider.enabled;
	public virtual bool IsInteractable => Collider.enabled;

	public virtual void StartObserve()
	{
		ControlUI.targetPoint.ShowPoint();
	}
	public virtual void Observe() { }
	public virtual void EndObserve()
	{
		ControlUI.targetPoint.HidePoint();
	}

	public virtual void Interact() {
	}

	public void ColliderEnable(bool trigger)
	{
		Collider.enabled = trigger;
	}
}



public enum InteractionType
{
	Failed,
	Press,
	Hold,
}