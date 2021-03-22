using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WindowItemInspector : WindowUI
{
    public UnityAction onTakeIt;
    public UnityAction onLeaveIt;

    [Space]
    [SerializeField] private TMPro.TextMeshProUGUI itemTittle;
    [SerializeField] private TMPro.TextMeshProUGUI itemDescription;

    [Space]
    [SerializeField] private Button takeItButton;
    [SerializeField] private Button leaveItButton;

    private void Awake()
    {
        takeItButton.onClick.AddListener(() => { onTakeIt?.Invoke(); });
        leaveItButton.onClick.AddListener(() => { onLeaveIt?.Invoke(); });
    }

    public void SetInformation(ItemData data)
	{
        itemTittle.text = data.name;
        itemDescription.text = data.description;
	}
}