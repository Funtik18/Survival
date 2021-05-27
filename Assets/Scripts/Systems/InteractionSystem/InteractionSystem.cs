public interface IObservable
{
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