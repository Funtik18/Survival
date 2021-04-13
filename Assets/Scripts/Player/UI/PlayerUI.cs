using UnityEngine;
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
	public Button buttonBreak;
	public GameObject panelBreak;

	public void Setup(Player player)
	{
		controlUI.Setup(player.Controller);

		conditionUI.Setup(player.Stats);

		windowsUI.itemInspectorWindow.Setup(player.itemInspector);
		windowsUI.buildingWindow.Setup(player.Build);
		windowsUI.harvestingWindow.Setup(player.Inventory);
		windowsUI.ignitionWindow.Setup(player.Inventory);


		windowsUI.backpackWindow.primaryContainer.SubscribeInventory(player.Inventory);
		windowsUI.backpackWindow.onBack += CloseInventory;

		windowsUI.harvestingWindow.onBack += CloseHarvesting;
		windowsUI.harvestingWindow.onHarvestingCompletely += OpenControlUI;

		windowsUI.ignitionWindow.onBack += CloseIgnition;
		windowsUI.ignitionWindow.onIgnitionCompletely += OpenControlUI;

		windowsUI.fireMenuWindow.onBack += CloseFireMenu;
    }

	public void OpenInventory()
	{
		CloseControlUI();
		CloseConditionUI();
		windowsUI.backpackWindow.ShowBackpackInspector();
	}
	public void CloseInventory()
	{
		windowsUI.backpackWindow.HideBackpack();
		OpenConditionUI();
		OpenControlUI();
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

	public void OpenIgnition(FireBuilding fireBuilding)
    {
		CloseControlUI();
		CloseConditionUI();
		windowsUI.ignitionWindow.SetBuilding(fireBuilding);
	}
	public void CloseIgnition()
    {
		windowsUI.ignitionWindow.HideWindow();
		OpenControlUI();
		OpenConditionUI();
	}

	public void OpenFireMenu(CamfireBuilding camfireBuilding)
    {
		CloseControlUI();
		CloseConditionUI();
		windowsUI.fireMenuWindow.Setup(GeneralAvailability.PlayerInventory, camfireBuilding);
	}
	public void CloseFireMenu()
    {
		windowsUI.fireMenuWindow.HideWindow();
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

	public void OpenResting()
    {
		CloseControlUI();
		windowsUI.restingWindow.ShowWindow();
	}
	public void CloseResting()
    {

    }


	public Button ShowBreakButton()
    {
		panelBreak.SetActive(true);
		return buttonBreak;
    }
	public Button HideBreakButton()
    {
		panelBreak.SetActive(false);
		return buttonBreak;
	}
}