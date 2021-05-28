using UnityEngine;
using UnityEngine.Events;

public class CookingSlot : MonoBehaviour
{
    public UnityAction onSlotEndWork;

    private FireBuilding owner;
    private ItemObject itemObject;

    public bool IsWorking => owner.IsEnable;

    public bool IsEmpty => itemObject == null;

    public void SetOwner(FireBuilding fire)
    {
        this.owner = fire;
    }

    public void SetItem(ItemObject itemObject)
    {
        this.itemObject = itemObject;
        this.itemObject.onDisable += DisposeSlot;

        if (itemObject is ItemObjectLiquidContainer itemObjectLiquid)
        {
            itemObjectLiquid.SetSlot(this);
        }

        UpdatePosition();
    }

    public void StartWork()
    {
        
    }
    public void Work()
    {
        if (IsEmpty) return;
        itemObject.UpdateItem();
    }
    public void EndWork()
    {
        onSlotEndWork?.Invoke();
    }

    private void UpdatePosition()
    {
        itemObject.transform.SetParent(transform);
        itemObject.transform.localPosition = Vector3.zero;
    }


    private void DisposeSlot()
    {
        itemObject = null;
        onSlotEndWork = null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);

        Gizmos.DrawWireSphere(transform.position, 0.2f);
    }
}