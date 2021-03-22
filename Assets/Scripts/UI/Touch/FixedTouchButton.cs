using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FixedTouchButton : PointerTouch 
{
    [SerializeField] private Image button;

    public override void OnPointerDown(PointerEventData eventData)
    {
        if(!IsEnable) return;

        base.OnPointerDown(eventData);

        button.color = new Color(0.7f, 0.7f, 0.7f, button.color.a);
    }


    public override void OnPointerUp(PointerEventData eventData)
    {
        if(!IsEnable) return;

        base.OnPointerUp(eventData);

        button.color = new Color(1, 1, 1, button.color.a);
    }


    public void IsActive(bool trigger)
	{
        gameObject.SetActive(trigger);
	}
}