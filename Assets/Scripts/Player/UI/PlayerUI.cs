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
	[Space]
	public TargetPoint targetPoint;
	public BlockPanel blockPanel;
	public SleepPanel sleepPanel;
	public ButtonBreak breakButton;
	public SavingPanel savingPanel;


	public void Setup(Player player)
	{
		controlUI.Setup(player.Controller);

		conditionUI.Setup(player.Status);

		windowsUI.buildingWindow.Setup(player.Build);
		windowsUI.harvestingWindow.Setup(player.Inventory);

		windowsUI.backpackWindow.onBack += CloseInventory;

		windowsUI.harvestingWindow.onBack += CloseHarvesting;
		windowsUI.harvestingWindow.onHarvestingCompletely += OpenControlUI;//edit

		windowsUI.ignitionWindow.onBack += CloseIgnition;

		windowsUI.fireMenuWindow.onBack += CloseFireMenu;

		windowsUI.restingWindow.onBack += CloseResting;

		windowsUI.exchangerWindow.onBack += CloseExchanger;
	}

	public void OpenInventory()
	{
		CloseControlUI();
		CloseConditionUI();
		windowsUI.backpackWindow.ShowBackpackInspector();
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
	public void OpenItemInspector(ItemObject itemObject)
	{
		CloseControlUI();
		CloseConditionUI();

		windowsUI.itemInspectorWindow.SetItem(itemObject);
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
	public void CloseResting()
    {
		windowsUI.restingWindow.HideWindow();
		OpenControlUI();
    }


	public void OpenExchander(float min, float max, float step, UnityAction<float> ok = null, UnityAction all = null, UnityAction cancel = null)
    {
		CloseControlUI();
		CloseConditionUI();

		windowsUI.exchangerWindow.Setup(min, max, step, ok, all, cancel).ShowWindow();

	}
	public void CloseExchanger()
    {
		windowsUI.exchangerWindow.HideWindow();

		OpenControlUI();
		OpenConditionUI();
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
}