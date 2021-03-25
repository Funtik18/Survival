using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine;

using Sirenix.OdinInspector;

public class ContainerGridUI : MonoBehaviour
{
	public UnityAction<Item> onItemChoosen;
	private UnityAction onSlotsChanged;

	[SerializeField] private ScrollRect scrollrect;
	[SerializeField] private GridLayoutGroup gridLayoutGroup;
	[AssetList]
	[SerializeField] private ContainerSlotUI slotPrefab;

	[ListDrawerSettings(ShowIndexLabels = true, DraggableItems = false)]
	[SerializeField] private List<ContainerSlotUI> slots = new List<ContainerSlotUI>();

	private Transform trans;
	public Transform Transform
    {
        get
        {
			if (trans == null)
				trans = transform;
			return trans;
        }
    }

	private int CurrentColumns => gridLayoutGroup.constraintCount;
	private int CurrentRows => Mathf.CeilToInt((float)slots.Count / CurrentColumns);
	private int constRows = 4;

	private Scrollbar VerticalScrollbar
	{
		get => scrollrect.verticalScrollbar;
	}

    private void Awake()
    {
		onSlotsChanged += UpdateScrollBarSteps;
		UpdateSlots();
	}

    public void PutItemsList(List<Item> items)
    {
		DisposeSlots();

		int rows = Mathf.CeilToInt((float)items.Count / CurrentColumns);

		//Debug.LogError(CurrentRows + " --- " + rows + " = " + items.Count + " / " + CurrentColumns);

		if(rows > 4)
        {
			int diff = rows - CurrentRows;
			if (diff == 0)//всовывается без проблем
			{
				PutItems(items);
			}
			else if(diff > 0)//нужно больше строк
			{
				AddRows(diff);
				PutItems(items);
			}
			else//нужно удалить не нужные строки до 4
			{
				RemoveRows(-diff);
				PutItems(items);
			}
        }
		else//всовывается без проблем в 4 строки
		{
			PutItems(items);
		}
    }

    #region Private
    private void PutItems(List<Item> items)
    {
		for (int i = 0; i < items.Count; i++)
		{
			PutItem(items[i]);
		}
	}
	private void PutItem(Item item)
	{
		for(int i = 0; i < slots.Count; i++)
		{
			if(slots[i].IsEmpty)
			{
				slots[i].SetItem(item);
				break;
			}
		}
    }

	private void DisposeSlots()
    {
        for (int i = 0; i < slots.Count; i++)
        {
			DisposeSlot(slots[i]);
        }
	}
	private void DisposeSlot(ContainerSlotUI slot)
	{
		slot.SetItem(null);
	}

    private void AddRows(int count)
    {
		for (int i = 0; i < count; i++)
		{
			AddRow();
		}
	}
	private void AddRow()
	{
		for(int i = 0; i < gridLayoutGroup.constraintCount; i++)
		{
			AddSlot(Instantiate(slotPrefab, Transform));
		}

		onSlotsChanged?.Invoke();
	}
	private ContainerSlotUI AddSlot(ContainerSlotUI slot, bool disposeSlot = false)
	{
		if (disposeSlot)
			DisposeSlot(slot);

		slot.onClick = ChooseSlot;
		slots.Add(slot);
		return slot;
	}

	private void RemoveRows(int count)
    {
		for (int i = 0; i < count; i++)
		{
			RemoveRow();
		}
	}
	private void RemoveRow()
	{
		List<GameObject> row = new List<GameObject>();

		for(int i = 0; i < gridLayoutGroup.constraintCount; i++)//check
		{
			GameObject go = transform.GetChild(transform.childCount - 1).gameObject;
			ContainerSlotUI slot = go.GetComponent<ContainerSlotUI>();
			slot.onClick = null;
			slots.Remove(slot);
			go.transform.SetParent(null);
			row.Add(go);
		}

		for(int i = 0; i < row.Count; i++)
		{
			Destroy(row[i]);
		}

		onSlotsChanged?.Invoke();
	}

    private void UpdateScrollBarSteps()
	{
		VerticalScrollbar.numberOfSteps = (CurrentRows - constRows) + 1;
	}

	private void UpdateSlots()
    {
        for (int i = 0; i < slots.Count; i++)
        {
			slots[i].onClick = ChooseSlot;
        }
    }

	private void ChooseSlot(Item data)
	{
		onItemChoosen?.Invoke(data);
	}

	[Button]
	private void GetSlots()
    {
		slots.Clear();
		for (int i = 0; i < transform.childCount; i++)
		{
			AddSlot(transform.GetChild(i).GetComponent<ContainerSlotUI>(), true);
		}
	}
    #endregion
}