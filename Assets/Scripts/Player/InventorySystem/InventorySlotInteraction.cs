using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class InventorySlotInteraction : MonoBehaviour, IPointerClickHandler 
{
	public UnityAction onClick;

	public void OnPointerClick(PointerEventData eventData)
	{
		onClick?.Invoke();
	}
}