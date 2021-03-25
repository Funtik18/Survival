using UnityEngine;

public class PlayerUI : MonoBehaviour
{
	public PlayerControlUI controlUI;
	public WindowsUI windowsUI;

	public void Setup(Player player)
	{
		windowsUI.backpackWindow.primaryContainer.Setup(player.Inventory);
		windowsUI.backpackWindow.primaryContainer.onItemChoosen += windowsUI.backpackWindow.itemInspector.SetItem;

		windowsUI.backpackWindow.onBack += CloseInventory;
		controlUI.buttonInventory.onClicked.AddListener(OpenInventory);//скрыть

		//controlUI.buttonPickUp.onClicked.AddListener(InspectorLook);

		//controlUI.buttonSpeedUp.onPressed.AddListener(() => { speedUp = true; });
		//controlUI.buttonSpeedUp.onUnPressed.AddListener(() => { currentSpeed = maxWalkSpeed; speedUp = false; });

		//controlUI.endurance.Setup(player.stats.Endurance);
	}

	private void OpenInventory()
	{
		Player.Instance.Lock();
		windowsUI.backpackWindow.ShowWindow();
	}
	private void CloseInventory()
	{
		Player.Instance.UnLock();
		windowsUI.backpackWindow.HideWindow();
	}
}