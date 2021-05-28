using Sirenix.OdinInspector;

using UnityEngine;

public class ButtonBreak : MonoBehaviour
{
    [SerializeField] private Pointer breakPointer;
    public Pointer BreakPointer => breakPointer;
    public bool IsEnable => breakPointer.gameObject.activeSelf;


    public void Enable(bool trigger)
    {
        breakPointer.gameObject.SetActive(trigger);
    }

    [Button]
    public void Enable()
    {
        Enable(true);
    }
    [Button]
    public void Disable()
    {
        Enable(false);
    }
}