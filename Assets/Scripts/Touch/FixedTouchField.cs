using UnityEngine;
using UnityEngine.EventSystems;

public class FixedTouchField : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Vector2 Direction;

    [HideInInspector] public Vector2 pointerOld;
    [HideInInspector] protected int pointerId;
    [HideInInspector] public bool isPressed;

    void Update()
    {
        if(isPressed)
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
            Direction = new Vector2();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        pointerId = eventData.pointerId;
        pointerOld = eventData.position;
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
    }
}
