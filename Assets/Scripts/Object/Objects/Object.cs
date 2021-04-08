using UnityEngine;

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

/// <summary>
/// Мировой объект, на котороый можно только посмотреть
/// </summary>
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

/// <summary>
///	Мировой объект, это объект с которым можно взаимодействовать
/// </summary>
public abstract class WorldObject : Object, IPerceptible
{
	public virtual bool IsObservable => Collider.enabled;
	public virtual bool IsInteractable => Collider.enabled;

	private InteractionButton button;
	protected InteractionButton InteractionButton
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

public abstract class WorldBoard<SD> : WorldBoard where SD : ObjectSD
{
	public SD data;

	public override void StartObserve()
	{
		base.StartObserve();
		GeneralAvailability.TargetPoint.SetToolTipText(data.description).ShowToolTip();
	}
}
public abstract class WorldObject<SD> : WorldObject where SD : ObjectSD
{
	public SD data;

	public override void StartObserve()
	{
		base.StartObserve();
		GeneralAvailability.TargetPoint.SetToolTipText(data.name).ShowToolTip();
	}
}