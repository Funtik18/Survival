using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;

public class InspectorUI : MonoBehaviour
{
    [OnValueChanged("Switch2D3D")]
    [SerializeField] private bool is2D = true;

    [Title("2D")]
    [SerializeField] private GameObject image;
    [SerializeField] private Image itemImage;
    [Title("3D")]
    [SerializeField] private GameObject rawImage;
    [SerializeField] private RawImage rawitemImage;
    [SerializeField] private ItemView3D view3D;

    public void InstantiateModel(ItemObject itemObject)//3D
    {
        if (is2D != false)
        {
            is2D = false;
            Switch2D3D();
        }

        view3D.InstantiateModel(itemObject);
        rawitemImage.enabled = true;
    }
    public void InstantiateModel(ItemSD item)//2D
    {
        if(is2D != true)
        {
            is2D = true;
            Switch2D3D();
        }

        itemImage.sprite = item.itemSprite;
        itemImage.enabled = true;
    }
    public void Dispose()
    {
        if (is2D)
        {
            itemImage.enabled = false;
            itemImage.sprite = null;
        }
        else
        {
            Debug.LogError("HERE");
            rawitemImage.enabled = false;
            view3D.DisposePlace();
        }
    }

    private void Switch2D3D()
    {
        image.SetActive(is2D);
        rawImage.SetActive(!is2D);
    }
}