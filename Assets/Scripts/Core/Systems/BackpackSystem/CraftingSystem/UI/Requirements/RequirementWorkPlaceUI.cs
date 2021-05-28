using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.UI;

public class RequirementWorkPlaceUI : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TMPro.TextMeshProUGUI text;

    [SerializeField] private WorkPlace current;

    [ShowIf("current", WorkPlace.Any)]
    [SerializeField] private Sprite any;
    [ShowIf("current", WorkPlace.Any)]

    [ShowIf("current", WorkPlace.WorkBench)]
    [SerializeField] private Sprite workbench;
    [ShowIf("current", WorkPlace.WorkBench)]

    private bool access = true;

    public void SetWorkPlace(WorkPlace workPlace, bool access)
    {
        current = workPlace;
        this.access = access;

        UpdateUI();
    }

    private void UpdateUI()
    {
        if(current == WorkPlace.Any)
        {
            image.sprite = any;
            text.text = "ANYWHERE";
        }
        else if(current == WorkPlace.WorkBench)
        {
            image.sprite = workbench;
            text.text = "WORKBENCH";
        }

        if (access)
        {
            image.color = Color.white;
            text.color = Color.white;
        }
        else
        {
            image.color = Color.red;
            text.color = Color.red;
        }
    }


    [Button]
    private void Check()
    {
        UpdateUI();
    }
}