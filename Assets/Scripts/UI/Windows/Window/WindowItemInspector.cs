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
        takeItButton.onClick.AddListener(TakeIt);
        leaveItButton.onClick.AddListener(LeaveIt);
    }

    public void Setup(ItemInspector inspector)
    {
        onTakeIt += inspector.ItemTake;
        onLeaveIt += inspector.ItemLeave;
    }

    public void SetInformation(ItemSD data)
	{
        itemTittle.text = data.name;
        itemDescription.text = data.description;
	}

    private void TakeIt()
    {
        onTakeIt?.Invoke();
    }
    private void LeaveIt()
    {
        onLeaveIt?.Invoke();
    }
}