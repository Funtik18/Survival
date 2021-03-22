using UnityEngine;

public class PlayerUI : MonoBehaviour
{
	public PlayerControlUI controlUI;
	public PlayerWindowsUI windowsUI;

	public void Setup(Player player)
	{
		//controlUI.buttonPickUp.onClicked.AddListener(InspectorLook);


		//controlUI.buttonSpeedUp.onPressed.AddListener(() => { speedUp = true; });
		//controlUI.buttonSpeedUp.onUnPressed.AddListener(() => { currentSpeed = maxWalkSpeed; speedUp = false; });

		windowsUI.inventoryWindow.onBack += CloseInventory;

		controlUI.buttonInventory.onClicked.AddListener(OpenInventory);

		//controlUI.endurance.Setup(player.stats.Endurance);
	}

	private void OpenInventory()
	{
		controlUI.BlockControl();
		windowsUI.inventoryWindow.ShowWindow();
	}
	private void CloseInventory()
	{
		controlUI.UnBlockControl();
		windowsUI.inventoryWindow.HideWindow();
	}
}