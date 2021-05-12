using UnityEngine;

public class InspectorUI : MonoBehaviour
{
    [SerializeField] private ItemView3D view3D;

    public void InstantiateModel(ItemObject itemObject)
    {
        view3D.InstantiateModel(itemObject);
    }
    public void Dispose()
    {
        view3D.DisposePlace();
    }
}