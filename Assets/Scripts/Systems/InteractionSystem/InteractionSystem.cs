using System.Collections;
using System.Collections.Generic;
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