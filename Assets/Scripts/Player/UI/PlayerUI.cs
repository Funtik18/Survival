using UnityEngine;

public class PlayerUI : MonoBehaviour
{
	public PlayerControlUI controlUI;
	public WindowsUI windowsUI;

	public void Setup(Player player)
	{
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

        //controlUI.buttonSpeedUp.onPressed.AddListener(() => { speedUp = true; });
        //controlUI.buttonSpeedUp.onUnPressed.AddListener(() => { currentSpeed = maxWalkSpeed; speedUp = false; });

        //controlUI.endurance.Setup(player.stats.Endurance);
    }

	public void OpenInventory()
	{
		CloseControlUI();
		windowsUI.backpackWindow.ShowBackpackInspector();
	}
	public void CloseInventory()
	{
		windowsUI.backpackWindow.HideBackpack();
		OpenControlUI();
	}

	public void OpenHarvesting(HarvestingObject harvesting)
    {
		CloseControlUI();
		windowsUI.harvestingWindow.SetHarvesting(harvesting);
	}
	public void CloseHarvesting()
    {
		windowsUI.harvestingWindow.HideWindow();
		OpenControlUI();
	}

	public void OpenIgnition(FireBuilding fireBuilding)
    {
		CloseControlUI();
		windowsUI.ignitionWindow.SetBuilding(fireBuilding);
	}
	public void CloseIgnition()
    {
		windowsUI.ignitionWindow.HideWindow();
		OpenControlUI();
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
}