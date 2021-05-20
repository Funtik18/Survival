using System.Collections;
using UnityEngine;

[System.Serializable]
public class PlayerOpportunities
{
	[SerializeField] private float useTimeByKg = 15f;
	[SerializeField] private Transform leftHand;
	[SerializeField] private Transform rightHand;

	private StatThirst thirst;
	private StatHungred hungred;

	private Player owner;
	private Inventory inventory;

	private WindowCondition condition;


	private Coroutine useCoroutine = null;
	public bool IsUseProccess => useCoroutine != null;

	public void Setup(Player player)
	{
		this.owner = player;
		this.inventory = player.Inventory;

		condition = GeneralAvailability.PlayerUI.conditionUI.conditionWindow;

		thirst = player.Status.stats.Thirst;
		hungred = player.Status.stats.Hungred;
	}

	public void DropItem(Item item)
	{
		ItemDataWrapper itemData = item.itemData;
		if (itemData.CurrentStackSize > 1)
		{
			//if (itemData.CurrentStackSize > 4)
			//	exchanger.SetItem(item);
			//else
			//	RemoveItem(item, 1);
		}
		else
			RemoveItem(item);
	}
	public void DestroyItem(Item item)
    {
		inventory.RemoveItem(item);
    }


	public void UseItem(Item item)
	{
		ItemSD data = item.itemData.scriptableData;

		if (data is ConsumableItemSD consuable)
		{

			if (IsCanGetIt(consuable))
			{
				OpenUI();
				useCoroutine = owner.StartCoroutine(Use(item, consuable));
			}
			else
			{
				Debug.LogError("I can not take it");
			}
		}
		else if(data is ToolWeaponSD weapon)
        {
            if (IsEquiped(item.itemData))
            {
				UnEquip();
            }
            else
            {
				EquipItem(item);
            }
        }
	}
	#region Use
	private IEnumerator Use(Item item, ConsumableItemSD consuable)
	{
		ItemDataWrapper data = item.itemData;

		if(consuable is WaterItemSD water)
			yield return UseWater(data, consuable.hydration);
		else
			yield return UseConsuable(data, consuable.calories, consuable.hydration);

		DestroyItem(item);

		StopUse();
	}
	private IEnumerator UseConsuable(ItemDataWrapper data, float maxCalories, float hydration)
    {
		//ui
		condition.thirst.EnableCondition(true);
		condition.hungred.EnableCondition(true);

		//lerp parametrs
		float currentCalories = data.CurrentCalories;
		Debug.LogError(currentCalories + "  " + maxCalories);
		float maxHydration = (currentCalories / maxCalories) * (thirst.Value * (hydration / 100f));

		float startCalories = hungred.CurrentValue;
		float startHydration = thirst.CurrentValue;

		float endCalories = startCalories + currentCalories;
		float endHydration = startHydration + maxHydration;

		float startWeight = data.CurrentWeight;
		float endWeight = 0.05f;

		float time = 0;
		float duration = useTimeByKg * data.CurrentWeight;//1kg

		Debug.LogError(startCalories + "  " + endCalories);
		Debug.LogError(startHydration + "  " + endHydration);
		//time duration cycle
		while (time < duration)
		{
			float normalStep = time / duration;

			thirst.CurrentValue = Mathf.Lerp(startHydration, endHydration, normalStep);
			hungred.CurrentValue = Mathf.Lerp(startCalories, endCalories, normalStep);
			data.CurrentCalories = Mathf.Lerp(currentCalories, 0, normalStep);
			data.CurrentWeight = Mathf.Lerp(startWeight, endWeight, normalStep);

			time += Time.deltaTime;

			GeneralAvailability.PlayerUI.barHight.UpdateFillAmount(normalStep, "%");

			if (thirst.IsFull && hungred.IsFull)
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
		condition.thirst.EnableCondition(true);

		//lerp parametrs
		float startWeight = data.CurrentWeight;
		float endWeight = data.MinimumWeight;

		float maxHydration = (startWeight - endWeight) / (thirst.Value * (hydration / 100f));//максимально возможное насыщение

		float startHydration = thirst.CurrentValue;
		float endHydration = startHydration + maxHydration;

		float time = 0;
		float duration = useTimeByKg * ((startWeight - endWeight));//1L == 1kg

		//time duration cycle
		while(time < duration)
        {
			float normalStep = time / duration;

			thirst.CurrentValue = Mathf.Lerp(startHydration, endHydration, normalStep);
			data.CurrentWeight = Mathf.Lerp(startWeight, endWeight, normalStep);

			time += Time.deltaTime;

			GeneralAvailability.PlayerUI.barHight.UpdateFillAmount(normalStep, "%");

			if (thirst.IsFull)
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

			CloseUI();
		}
	}
    
	private bool IsCanGetIt(ConsumableItemSD consuable)
    {
		if(consuable is WaterItemSD)
        {
			if (thirst.IsFullNear)
				return false;
        }
        else
        {
			if (thirst.IsFullNear && hungred.IsFullNear)
				return false;
        }
		return true;
    }
	#endregion

	#region Actions
	private ItemObjectWeapon weapon;

	public bool IsEquiped(ItemDataWrapper data)
    {
		if (weapon == null) return false;
		return weapon.Data == data;
	}

	public void EquipUnEquip(Item item)
    {
		if(item != null)
        {
			bool isNewWeapon = weapon == null ? true : item.itemData != weapon.Data;

			UnEquip();
			if (isNewWeapon)
				EquipItem(item);
		}
	}

    public void EquipItem(Item item)
    {
		if(item != null)
        {
			ItemObject itemObject = item.itemData.scriptableData.model;

			if (itemObject is ItemObjectWeapon)
			{
				weapon = ObjectPool.GetObject(itemObject.gameObject).GetComponent<ItemObjectWeapon>();
				weapon.ColliderEnable(false);

				PutInHand(weapon.transform, rightHand);

				GeneralAvailability.PlayerUI.controlUI.windowShoting.Setup(weapon, Aim, DeAim, Shoot, Reload, UnEquip);
				GeneralAvailability.PlayerUI.OpenShooting();
			}
		}
    }
	public void FreeHands()
    {
		if(weapon != null)
        {
			ObjectPool.ReturnGameObject(weapon.gameObject);
			weapon.transform.SetParent(null);
			weapon.ColliderEnable(true);

			GeneralAvailability.PlayerUI.CloseShooting();
		}

		weapon = null;
	}
	private void PutInHand(Transform obj, Transform parent)
    {
		obj.SetParent(parent);
		obj.localPosition = Vector3.zero;
		obj.localRotation = Quaternion.identity;
	}
	private void Aim()
    {
		weapon.Aim();
	}
	private void DeAim()
	{
		weapon.DeAim();
	}
	private void Shoot()
    {
		weapon.Shoot();
	}
	private void Reload()
    {

	}
	private void UnEquip()
    {
		FreeHands();
	}
	#endregion

	public void ActionsItem(Item item)
    {

    }

	private void OpenUI()
    {
		GeneralAvailability.PlayerUI.blockPanel.Enable(true);
		GeneralAvailability.PlayerUI.ShowBreakButton().BreakPointer.AddPressListener(StopUse);
		GeneralAvailability.PlayerUI.barHight.UpdateFillAmount(0, "%").ShowBar();

	}
	private void CloseUI()
    {
		condition.hungred.EnableCondition(false);
		condition.thirst.EnableCondition(false);

		GeneralAvailability.PlayerUI.barHight.HideBar();
		GeneralAvailability.PlayerUI.HideBreakButton();
		GeneralAvailability.PlayerUI.blockPanel.Enable(true);
	}


    private void RemoveItem(Item item, int count)
	{
		inventory.RemoveItem(item, count);
		CreateWorldItem(item, count);
	}
	private void RemoveItem(Item item)
	{
		RemoveItem(item, item.itemData.CurrentStackSize);
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