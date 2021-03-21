using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FloatingJoystick : Joystick
{
	public override bool IsEnable 
    { 
        get => base.IsEnable;
        set
        {
            background.gameObject.SetActive(false);
            base.IsEnable = value;
        }
    }

	protected override void Start()
    {
        base.Start();
        background.gameObject.SetActive(false);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if(!IsEnable) return;
        
        background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
        background.gameObject.SetActive(true);
        base.OnPointerDown(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if(!IsEnable) return;

        background.gameObject.SetActive(false);
        base.OnPointerUp(eventData);
    }
}