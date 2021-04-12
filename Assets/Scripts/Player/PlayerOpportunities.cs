using System.Collections;
using UnityEngine;

[System.Serializable]
public class PlayerOpportunities
{
	private StatThirst thirst;
	private StatHungred hungred;

	private Player owner;
	private PlayerUI ui;
	private Inventory inventory;

	private Coroutine useCoroutine = null;
	public bool IsUseProccess => useCoroutine != null;

	public void Setup(Player player)
	{
		this.owner = player;
		this.ui = player.UI;
		this.inventory = player.Inventory;

		thirst = player.Stats.Thirst;
		hungred = player.Stats.Hungred;
	}

	public void DropItem(Item item)
	{
		ItemDataWrapper itemData = item.itemData;
		if (itemData.StackSize > 1)
		{
			if (itemData.StackSize > 4)
			{
				ui.windowsUI.exchangerWindow.SetItem(item);
			}
			else
			{
				inventory.RemoveItem(item, 1);
			}
		}
		else
		{
			inventory.RemoveItem(item);
		}
	}

	public void UseItem(Item item)
	{
		ItemSD data = item.itemData.scriptableData;

		if (data is ConsuableItemSD consuable)
		{
			ui.ShowBreakButton().onClick.AddListener(StopUse);
			ui.barHight.ShowBar();

			useCoroutine = owner.StartCoroutine(Use(item, consuable));
		}
	}
	private IEnumerator Use(Item item, ConsuableItemSD consuable)
	{
		// onchange вызывается и обновляет айтем
		ItemDataWrapper data = item.itemData;

		ui.conditionUI.conditionWindow.hungred.EnableCondition(true);
		ui.conditionUI.conditionWindow.thirst.EnableCondition(true);

		float maxCalories = data.CurrentCalories;
		float maxHydration = (maxCalories / consuable.calories) * (thirst.Value * (consuable.hydration / 100f));

		float startCalories = hungred.CurrentValue;
		float startHydration = thirst.CurrentValue;

		float endCalories = startCalories + maxCalories;
		float endHydration = startHydration + maxHydration;

		float time = 0;
		float duration = 15f * (maxCalories / 1000f);

		while (time < duration)
		{
			float normalStep = time / duration;

			thirst.CurrentValue = Mathf.Lerp(startHydration, endHydration, normalStep);
			hungred.CurrentValue = Mathf.Lerp(startCalories, endCalories, normalStep);
			data.CurrentCalories = Mathf.Lerp(maxCalories, 0, normalStep);

			time += Time.deltaTime;

			ui.barHight.UpdateFillAmount(normalStep, "%");

			yield return null;
		}
		data.CurrentCalories = 0;

		thirst.CurrentValue = endHydration;
		hungred.CurrentValue = endCalories;

		ui.conditionUI.conditionWindow.hungred.EnableCondition(false);
		ui.conditionUI.conditionWindow.thirst.EnableCondition(false);

		DropItem(item);

		StopUse();
	}
	private void StopUse()
	{
		if (IsUseProccess)
		{
			owner.StopCoroutine(useCoroutine);
			useCoroutine = null;

			ui.barHight.HideBar();
			ui.HideBreakButton().onClick.RemoveAllListeners();
		}
	}
}