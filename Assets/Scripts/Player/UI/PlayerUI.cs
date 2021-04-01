using UnityEngine;

public class PlayerUI : MonoBehaviour
{
	public PlayerControlUI controlUI;
	public WindowsUI windowsUI;

	public void Setup(Player player)
	{
		windowsUI.itemInspectorWindow.Setup(player.itemInspector);
		windowsUI.buildingWindow.Setup(player.Build);

		windowsUI.backpackWindow.primaryContainer.SubscribeInventory(player.Inventory);
		windowsUI.backpackWindow.onBack += CloseInventory;

        //controlUI.buttonSpeedUp.onPressed.AddListener(() => { speedUp = true; });
        //controlUI.buttonSpeedUp.onUnPressed.AddListener(() => { currentSpeed = maxWalkSpeed; speedUp = false; });

        //controlUI.endurance.Setup(player.stats.Endurance);
    }

	public void OpenInventory()
	{
		GeneralAvailability.Player.Lock();
		windowsUI.backpackWindow.ShowBackpackInspector();
	}
	public void CloseInventory()
	{
		GeneralAvailability.Player.UnLock();
		windowsUI.backpackWindow.HideBackpack();
	}
}