using System.Collections;
using UnityEngine;

[System.Serializable]
public class PlayerOpportunities
{
	[SerializeField] private float useTimeKg = 15f;
	[SerializeField] private Times useTimeKG;
	[Space]
	[SerializeField] private Transform leftHand;
	[SerializeField] private Transform rightHand;

	//chash
	private StatThirst thirst;
	private StatHungred hungred;

	private Player owner;
	private Inventory inventory;

	private WindowCondition condition;

	public bool IsUseProccess { get; private set; }

	public void Setup(Player player)
	{
		this.owner = player;
		this.inventory = player.Inventory;

		condition = GeneralAvailability.PlayerUI.conditionUI.conditionWindow;
		windowShooting = GeneralAvailability.PlayerUI.controlUI.windowShoting;


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




	public void UseItem(Item item)
	{
		itemUse = item;
		itemUseData = itemUse.itemData;
		ItemSD data = itemUseData.scriptableData;

		if (data is ConsumableItemSD consumable)
			UseConsumableItem(consumable);
		else if (data is ToolItemSD tool)
			UseTool(tool);
	}

	private void UseConsumableItem(ConsumableItemSD consumable)
	{
		consumableItem = consumable;

		if (IsCanGetIt())
		{
			IsUseProccess = true;

			itemUseData = itemUse.itemData;

			float duration = useTimeKg * itemUseData.CurrentBaseWeight;//1kg
			Times skip = new Times();
			skip.TotalSeconds = (int)(useTimeKG.TotalSeconds * itemUseData.CurrentBaseWeight);

			GeneralTime.Instance.SkipSetup(start: StartUseConsumable, progress: UpdateUseConsumable, completely: CompletelyUseConsumable).StartSkip(skip, duration);
		}
	}
	private void UseTool(ToolItemSD tool)
	{
		if (tool is ToolWeaponSD weapon)
		{
			if (IsEquiped(itemUseData))
			{
				UnEquip();
			}
			else
			{
				EquipItem(itemUse);
			}
		}
	}


	#region Use
	private Item itemUse;
	private ItemDataWrapper itemUseData;
	private ConsumableItemSD consumableItem;
	private float startHydration, endHydration;
	private float itemHydration;
	private float startCalories, endCalories;
	private float itemStartCalories;
	private float itemStartWeight, itemEndWeight;

	private void StartUseConsumable()
    {
		OpenUI();

		//lerp parametrs
		itemStartWeight = itemUseData.CurrentBaseWeight;
		itemEndWeight = 0f;

		itemStartCalories = itemUseData.CurrentCalories;

		if(consumableItem is WaterItemSD)
        {
			itemHydration = (itemStartWeight - itemEndWeight) / (thirst.Value * (consumableItem.hydration / 100f));//максимально возможное насыщение
        }
        else
        {
			itemHydration = (itemStartCalories / consumableItem.calories) * (thirst.Value * (consumableItem.hydration / 100f));
		}

		startCalories = hungred.CurrentValue;
		endCalories = startCalories + itemStartCalories;

		startHydration = thirst.CurrentValue;
		endHydration = startHydration + itemHydration;
	}
	private void UpdateUseConsumable(float progress)
    {
		GeneralAvailability.PlayerUI.barHight.UpdateFillAmount(progress, "%");

		hungred.CurrentValue = Mathf.Lerp(startCalories, endCalories, progress);
		thirst.CurrentValue = Mathf.Lerp(startHydration, endHydration, progress);

		itemUseData.CurrentCalories = Mathf.Lerp(itemStartCalories, 0, progress);
		itemUseData.CurrentBaseWeight = Mathf.Lerp(itemStartWeight, itemEndWeight, progress);

		if (!IsCanGetIt())
		{
			BreakUseConsumable();
		}
	}
	private void BreakUseConsumable()
    {
		GeneralTime.Instance.BreakSkipTime();

		IsUseProccess = false;

		CloseUI();
	}
	private void CompletelyUseConsumable()
    {
		itemUseData.CurrentCalories = 0;
		itemUseData.CurrentBaseWeight = itemEndWeight;

		hungred.CurrentValue = endCalories;
		thirst.CurrentValue = endHydration;

		RemoveItem(itemUse);

		IsUseProccess = false;

		CloseUI();

		CheckEffects();
	}
	private void CheckEffects()
    {
		//Effects
		if (consumableItem.isHaveEffects)
		{
			owner.Status.effects.AddEffects(consumableItem.effects);
		}
	}


	private bool IsCanGetIt()
    {
		if (thirst.IsFullNear && hungred.IsFullNear)
			return false;
		return true;
	}
	#endregion

	#region Actions
	private WindowShooting windowShooting;
	private ItemObjectWeapon weapon;

	private Coroutine reloadCoroutine = null;
	private bool IsReloadProccess => reloadCoroutine != null;

    public void EquipItem(Item item)
    {
		if(item != null)
        {
			ItemObject itemObject = item.itemData.scriptableData.model;

			if (itemObject is ItemObjectWeapon)
			{
				weapon = ObjectPool.GetObject(itemObject.gameObject).GetComponent<ItemObjectWeapon>();

				weapon.CurrentData = item.itemData;

				weapon.ColliderEnable(false);
				weapon.Enable(true);

				weapon.onRequiredReload += Reload;

				PutInHand(weapon.transform, rightHand);

				windowShooting.Setup(weapon, Aim, DeAim, Shoot, Reload, UnEquip);
				windowShooting.ShowWindow();
			}
		}
    }
	public void UnEquip()
	{
		if (weapon != null)
		{
			DeAim();
			BreakReload();
			FreeHands();
		}
		weapon = null;
	}
	public bool IsEquiped(ItemDataWrapper data)
    {
		if (weapon == null) return false;
		return weapon.CurrentData == data;
	}
	public void EquipUnEquip(Item item)
	{
		if (item != null)
		{
			bool isNewWeapon = weapon == null ? true : item.itemData != weapon.CurrentData;

			UnEquip();
			if (isNewWeapon)
				EquipItem(item);
		}
	}


	private void FreeHands()
    {
		weapon.Enable(false);

		ObjectPool.ReturnGameObject(weapon.gameObject);
		weapon.transform.SetParent(null);
		weapon.ColliderEnable(true);

		windowShooting.HideWindow();
	}
	private void PutInHand(Transform obj, Transform parent)
    {
		obj.SetParent(parent);
		obj.localPosition = Vector3.zero;
		obj.localRotation = Quaternion.identity;
	}

	private void Aim()
    {
		if (!IsReloadProccess)
        {
			windowShooting.Aim();
			weapon.Aim();

			GeneralAvailability.Player.Camera.SetFieldOfView(35, 0.7f);
			GeneralAvailability.Player.Controller.SetLookSensitivity(1f);
        }
        else
        {
			BreakReload();
			Aim();
		}
	}
	private void DeAim()
	{
		weapon.DeAim();
		windowShooting.DeAim();
		GeneralAvailability.Player.Camera.ResetFieldOfView(0.3f);
		GeneralAvailability.Player.Controller.ResetLookSensitivity();
	}

	private void Shoot()
    {
		if(!IsReloadProccess)
			weapon.Shoot();
	}
	private void Reload()
    {
		if(!weapon.IsFull)
			StartReload();
	}

    #region Reload
	private void StartReload()
    {
        if (!IsReloadProccess)
        {
			DeAim();
			reloadCoroutine = owner.StartCoroutine(ReLoad());
        }
    }
	private IEnumerator ReLoad()
    {
        while (!weapon.IsFull)
        {
			yield return new WaitForSeconds(weapon.AmmoReloadDelay);
			weapon.CurrentСlipCapacity += weapon.AmmoReloadRate;
        }

		StopReload();
	}
	private void BreakReload()
    {
		StopReload();
	}
	private void StopReload()
    {
		if (IsReloadProccess)
		{
			owner.StopCoroutine(reloadCoroutine);
			reloadCoroutine = null;
		}
	}
    #endregion
    #endregion

    public void ActionsItem(Item item)
    {

    }

	private void OpenUI()
    {
		GeneralAvailability.PlayerUI.blockPanel.Enable(true);
		GeneralAvailability.PlayerUI.ShowBreakButton().BreakPointer.AddPressListener(BreakUseConsumable);
		GeneralAvailability.PlayerUI.barHight.UpdateFillAmount(0, "%").ShowBar();

		condition.thirst.EnableCondition(true);
		condition.hungred.EnableCondition(true);
	}
	private void CloseUI()
    {
		condition.hungred.EnableCondition(false);
		condition.thirst.EnableCondition(false);

		GeneralAvailability.PlayerUI.barHight.HideBar();
		GeneralAvailability.PlayerUI.HideBreakButton();
		GeneralAvailability.PlayerUI.blockPanel.Enable(false);
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