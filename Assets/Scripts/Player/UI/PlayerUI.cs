using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
	public PlayerControlUI controlUI;
	public ConditionUI conditionUI;
	public WindowsUI windowsUI;
	[Space]
	public ProgressBar barLow;
	public ProgressBar barHight;
	[Space]
	public GameObject panelDead;
	[Space]
	public TargetPoint targetPoint;
	public BlockPanel blockPanel;
	public SleepPanel sleepPanel;
	public ButtonBreak breakButton;
	public SavingPanel savingPanel;

	[Space]
	public WindowExchanger exchangerWindow;

	public void Setup(Player player)
	{
		controlUI.Setup(player.Controller);

		conditionUI.Setup(player.Status);

		windowsUI.buildingWindow.Setup(player.Build);

		windowsUI.backpackWindow.onBack += CloseInventory;

		windowsUI.harvestingWindow.onBack += CloseHarvesting;

		windowsUI.harvestingCarcassWindow.onBack += CloseHarvestingCarcass;

		windowsUI.ignitionWindow.onBack += CloseIgnition;

		windowsUI.fireMenuWindow.onBack += CloseFireMenu;

		windowsUI.restingWindow.onBack += CloseResting;
	}

	public void OpenDeadPanel()
    {
		CloseControlUI();
		CloseConditionUI();
		panelDead.SetActive(true);
	}

	public void OpenInventory()
	{
		CloseControlUI();
		CloseConditionUI();
		windowsUI.backpackWindow.ShowBackpack();
	}
	public void OpenInventoryWithContainer(Inventory container = null)
    {
		CloseControlUI();
		CloseConditionUI();

		if(container != null)
			windowsUI.backpackWindow.SetSecondaryContainer(container);

		windowsUI.backpackWindow.ShowBackpackWithContainer();
	}
	public void OpenCrafting()
    {
		CloseControlUI();
		CloseConditionUI();
		windowsUI.backpackWindow.ShowBackpackCrafting();
	}
	public void CloseInventory()
	{
		windowsUI.backpackWindow.HideBackpack();
		OpenConditionUI();
		OpenControlUI();
	}


	public void OpenItemInspector(Item item)
    {
		CloseControlUI();
		CloseConditionUI();

		windowsUI.itemInspectorWindow.SetItem(item);
	}
	public void CloseItemInspector()
    {
		windowsUI.itemInspectorWindow.HideWindow();

		OpenControlUI();
		OpenConditionUI();
	}


	public void OpenHarvesting(HarvestingObject harvesting)
    {
		CloseControlUI();
		CloseConditionUI();
		windowsUI.harvestingWindow.SetHarvesting(harvesting);
	}
	public void CloseHarvesting()
    {
		windowsUI.harvestingWindow.HideWindow();
		OpenControlUI();
		OpenConditionUI();
	}

	public void OpenHarvestingCarcass(Animal harvesting)
	{
		CloseControlUI();
		CloseConditionUI();
		windowsUI.harvestingCarcassWindow.SetHarvesting(harvesting);
	}
	public void CloseHarvestingCarcass()
	{
		windowsUI.harvestingCarcassWindow.HideWindow();
		OpenControlUI();
		OpenConditionUI();
	}


	public void OpenIgnition()
    {
		CloseControlUI();
		CloseConditionUI();
		windowsUI.ignitionWindow.OpenWindow();
	}
	public void CloseIgnition()
    {
		windowsUI.ignitionWindow.HideWindow();
		OpenControlUI();
		OpenConditionUI();
	}


	public void OpenFireMenu(FireBuilding fireBuilding)
    {
		CloseControlUI();
		CloseConditionUI();
		windowsUI.fireMenuWindow.Setup(GeneralAvailability.PlayerInventory, fireBuilding);
	}
	public void CloseFireMenu()
    {
		windowsUI.fireMenuWindow.HideWindow();
		OpenControlUI();
		OpenConditionUI();
	}
	public void BreakFireMenu()
    {
		windowsUI.fireMenuWindow.Out();
		OpenControlUI();
		OpenConditionUI();
	}

	public void OpenRadialMenu()
    {
		controlUI.radialMenu.ShowWindow();
    }
	public void CloseRadialMenu()
    {
		controlUI.radialMenu.HideWindow();
	}


	public void OpenResting()
    {
		CloseControlUI();
		windowsUI.restingWindow.ShowWindow();
	}
	public void OpenPassTime()
    {
		CloseControlUI();
		windowsUI.restingWindow.ShowWindow();
	}
	public void OpeRestingForBag(UnityAction takeIt)
    {
		CloseControlUI();
		windowsUI.restingWindow.onTakeIt = takeIt;
		windowsUI.restingWindow.UseTakeButton();
		windowsUI.restingWindow.ShowWindow();
	}
	public void CloseResting()
    {
		windowsUI.restingWindow.HideWindow();
		OpenControlUI();
    }



	public void OpenConditionUI()
	{
		conditionUI.ShowStamina();
		conditionUI.ShowCondition();
	}
	public void CloseConditionUI()
	{
		conditionUI.HideStamina();
		conditionUI.HideCondition();
	}


	public void OpenControlUI()
	{
		GeneralAvailability.Player.UnLock();
		controlUI.ShowWindow();
	}
	public void CloseControlUI()
	{
		controlUI.HideWindow();
		GeneralAvailability.Player.Lock();
	}


	public ButtonBreak ShowBreakButton()
    {
		breakButton.gameObject.SetActive(true);
		return breakButton;
    }
	public void HideBreakButton()
    {
		breakButton.BreakPointer.RemoveAllListeners();
		breakButton.gameObject.SetActive(false);
	}



	public void OpenExchander(float min, float max, float step, UnityAction<float> ok = null, UnityAction all = null, UnityAction cancel = null)
	{
		exchangerWindow.Setup(min, max, step, ok, all, cancel).ShowWindow();
	}
	public void CloseExchanger()
	{
		exchangerWindow.HideWindow();
	}
}