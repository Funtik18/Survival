using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FixedTouchButton : PointerTouch 
{
    [SerializeField] private Image button;
    [SerializeField] private float maxAlpha = 0.7f;

    public override void OnPointerDown(PointerEventData eventData)
    {
        if(!IsEnable) return;

        base.OnPointerDown(eventData);

        PressButton();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if(!IsEnable) return;

        base.OnPointerUp(eventData);

        UnPressButton();
    }

    public virtual void PressButton()
    {
        button.color = new Color(0.7f, 0.7f, 0.7f, maxAlpha);
    }
    public virtual void UnPressButton()
    {
        button.color = new Color(1, 1, 1, maxAlpha);
    }
}