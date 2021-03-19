using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interaction : MonoBehaviour, IInteractable
{
	public UnityAction<Collider> onTriggerEnter;
	public UnityAction<Collider> onTriggerExit;


	public virtual void Interact()
	{
		Debug.LogError("Interect");
	}

	public virtual void OnTriggerEnter(Collider other)
	{
		onTriggerEnter?.Invoke(other);
	}
	public virtual void OnTriggerExit(Collider other)
	{
		onTriggerExit?.Invoke(other);
	}
}
