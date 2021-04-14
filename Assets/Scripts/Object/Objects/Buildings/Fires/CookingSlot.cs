using UnityEngine;

public class CookingSlot : MonoBehaviour
{
    [SerializeField] private Collider collider;

    private Item item;
    private ItemObject itemObject;


    public bool IsEmpty => item == null;

    public void SetItem(Item item)
    {
        this.item = item;

        InstantiateItem();
    }

    public void UpdateItem()
    {
        if (IsEmpty) return;

        Debug.LogError("Update");
    }


    private void InstantiateItem()
    {
        itemObject = Instantiate(item.itemData.scriptableData.model, transform);
        itemObject.transform.localPosition = Vector3.zero;

    }

    private void OnDrawGizmos()
    {
        if (!collider) return;

        Gizmos.color = new Color(1, 0, 0, 0.5f);

        Gizmos.DrawCube(transform.position, collider.bounds.size);
    }
}
