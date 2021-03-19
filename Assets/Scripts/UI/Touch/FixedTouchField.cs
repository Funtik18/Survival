using UnityEngine;
using UnityEngine.EventSystems;

public class FixedTouchField : PointerTouch
{
    public Vector2 Direction;

    [HideInInspector] public Vector2 pointerOld;
    [HideInInspector] protected int pointerId;

    void Update()
    {
        if(IsPressed)
        {
            if(pointerId >= 0 && pointerId < Input.touches.Length)
            {
                Direction = Input.touches[pointerId].position - pointerOld;
                pointerOld = Input.touches[pointerId].position;
            }
            else
            {
                Direction = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - pointerOld;
                pointerOld = Input.mousePosition;
            }
        }
        else
        {
            Direction = new Vector2();//переделать
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        pointerId = eventData.pointerId;
        pointerOld = eventData.position;
    }
}
