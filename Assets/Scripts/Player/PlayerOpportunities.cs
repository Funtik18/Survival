using System.Collections;
using UnityEngine;

[System.Serializable]
public class PlayerOpportunities
{
	[SerializeField] private float useTimeByKg = 15f;

	private StatThirst thirst;
	private StatHungred hungred;

	private Player owner;
	private PlayerUI ui;
	private Inventory inventory;

	private WindowExchanger exchanger;

	private Coroutine useCoroutine = null;
	public bool IsUseProccess => useCoroutine != null;

	public void Setup(Player player)
	{
		this.owner = player;
		this.ui = player.UI;
		this.inventory = player.Inventory;

		this.exchanger = ui.windowsUI.exchangerWindow;
		exchanger.onOk += RemoveItem;
		exchanger.onAll += RemoveItem;

		thirst = player.Stats.Thirst;
		hungred = player.Stats.Hungred;
	}

	public void DropItem(Item item)
	{
		ItemDataWrapper itemData = item.itemData;
		if (itemData.StackSize > 1)
		{
			if (itemData.StackSize > 4)
				exchanger.SetItem(item);
			else
				RemoveItem(item, 1);
		}
		else
			RemoveItem(item);
	}
	public void DestroyItem(Item item)
    {
		inventory.RemoveItem(item);
    }

    #region Use
    public void UseItem(Item item)
	{
		ItemSD data = item.itemData.scriptableData;

		if (data is ConsuableItemSD consuable)
		{
            if (IsCanGetIt(consuable))
            {
				ui.ShowBreakButton().onClick.AddListener(StopUse);
				ui.barHight.ShowBar();

				useCoroutine = owner.StartCoroutine(Use(item, consuable));
            }
            else
            {
				Debug.LogError("I can not take it");
            }
		}
	}
	private IEnumerator Use(Item item, ConsuableItemSD consuable)
	{
		ItemDataWrapper data = item.itemData;

		if(consuable is WaterItemSD water)
			yield return UseWater(data, consuable.hydration);
        else
			yield return UseConsuable(data, consuable.calories, consuable.hydration);

		DestroyItem(item);

		StopUse();
	}
	private IEnumerator UseConsuable(ItemDataWrapper data, float calories, float hydration)
    {
		//ui
		ui.conditionUI.conditionWindow.thirst.EnableCondition(true);
		ui.conditionUI.conditionWindow.hungred.EnableCondition(true);

		//lerp parametrs
		float maxCalories = data.CurrentCalories;
		float maxHydration = (maxCalories / calories) * (thirst.Value * (hydration / 100f));

		float startCalories = hungred.CurrentValue;
		float startHydration = thirst.CurrentValue;

		float endCalories = startCalories + maxCalories;
		float endHydration = startHydration + maxHydration;

		float startWeight = data.CurrentWeight;
		float endWeight = 0.05f;

		float time = 0;
		float duration = useTimeByKg * (maxCalories / 1000f);//1kg

		//time duration cycle
		while (time < duration)
		{
			float normalStep = time / duration;

			thirst.CurrentValue = Mathf.Lerp(startHydration, endHydration, normalStep);
			hungred.CurrentValue = Mathf.Lerp(startCalories, endCalories, normalStep);
			data.CurrentCalories = Mathf.Lerp(maxCalories, 0, normalStep);
			data.CurrentWeight = Mathf.Lerp(startWeight, endWeight, normalStep);

			time += Time.deltaTime;

			ui.barHight.UpdateFillAmount(normalStep, "%");

			if (thirst.IsConcreteFull && hungred.IsConcreteFull)
			{
				StopUse();
				yield break;
			}
			else
				yield return null;
		}

		//end
		data.CurrentCalories = 0;
		data.CurrentWeight = endWeight;

		thirst.CurrentValue = endHydration;
		hungred.CurrentValue = endCalories;
	}
	private IEnumerator UseWater(ItemDataWrapper data, float hydration)
    {
		//ui
		ui.conditionUI.conditionWindow.thirst.EnableCondition(true);

		//lerp parametrs
		float startWeight = data.CurrentWeight;
		float endWeight = data.MinimumWeight;

		float maxHydration = ((startWeight - endWeight) / (data.scriptableData.weight - endWeight)) * (thirst.Value * (hydration / 100f));

		float startHydration = thirst.CurrentValue;
		float endHydration = startHydration + maxHydration;

		float time = 0;
		float duration = useTimeByKg * ((startWeight - endWeight));//1L = 1kg

		//time duration cycle
		while(time < duration)
        {
			float normalStep = time / duration;

			thirst.CurrentValue = Mathf.Lerp(startHydration, endHydration, normalStep);
			data.CurrentWeight = Mathf.Lerp(startWeight, endWeight, normalStep);

			time += Time.deltaTime;
			
			ui.barHight.UpdateFillAmount(normalStep, "%");
			
			if (thirst.IsConcreteFull)
            {
				StopUse();
				yield break;
			}
			else
				yield return null;
		}

		//end
		data.CurrentWeight = endWeight;
		thirst.CurrentValue = endHydration;
	}

	private void StopUse()
	{
		if (IsUseProccess)
		{
			owner.StopCoroutine(useCoroutine);
			useCoroutine = null;

			ui.conditionUI.conditionWindow.hungred.EnableCondition(false);
			ui.conditionUI.conditionWindow.thirst.EnableCondition(false);

			ui.barHight.HideBar();
			ui.HideBreakButton().onClick.RemoveAllListeners();
		}
	}
    
	private bool IsCanGetIt(ConsuableItemSD consuable)
    {
		if(consuable is WaterItemSD)
        {
			if (thirst.IsFull)
				return false;
        }
        else
        {
			if (thirst.IsFull && hungred.IsFull)
				return false;
        }
		return true;
    }
	#endregion

    private void RemoveItem(Item item, int count)
	{
		inventory.RemoveItem(item, count);
		CreateWorldItem(item, count);
	}
	private void RemoveItem(Item item)
	{
		RemoveItem(item, item.itemData.StackSize);
	}
	private void CreateWorldItem(Item item, int count)
    {
  //      for (int i = 0; i < count; i++)
  //      {
		//	ItemObject worldItem = GameObject.Instantiate(item.itemData.scriptableData.model);
		//	worldItem.SetData(item.itemData);
		//	worldItem.transform.position = owner.transform.position;
		//}
	}
}