using UnityEngine;

public class CraftingItemInspectorUI : MonoBehaviour
{
    [SerializeField] private InspectorUI inspectorUI;

    [SerializeField] private TMPro.TextMeshProUGUI itemTittle;
    [SerializeField] private TMPro.TextMeshProUGUI itemDescription;

    //cash
    private ItemSD currentItemSD;

    public void SetItem(ItemSD itemSD)
    {
        currentItemSD = itemSD;

        UpdateUI();
    }

    public void UpdateUI()
    {
        if (currentItemSD)
        {
            itemTittle.text = currentItemSD.objectName;
            itemDescription.text = currentItemSD.description;

            //inspectorUI.InstantiateModel(currentItemSD.model);
            inspectorUI.InstantiateModel(currentItemSD);
        }
        else
        {
            itemTittle.text = "";
            itemDescription.text = "";

            inspectorUI.Dispose();
        }
    }
}