using UnityEngine;

public class PlayerUI : MonoBehaviour
{
	public PlayerControlUI controlUI;
	public WindowsUI windowsUI;

	public void Setup(Player player)
	{
		windowsUI.itemInspectorWindow.Setup(player.itemInspector);

		windowsUI.backpackWindow.primaryContainer.SubscribeInventory(player.Inventory);
		windowsUI.backpackWindow.onBack += CloseInventory;


		controlUI.buttonOpenRadialMenu.onPressed += OpenRadialMenu;
		controlUI.buttonCloseRadialMenu.onPressed += CloseRadialMenu;


        //controlUI.buttonPickUp.onClicked.AddListener(InspectorLook);

        //controlUI.buttonSpeedUp.onPressed.AddListener(() => { speedUp = true; });
        //controlUI.buttonSpeedUp.onUnPressed.AddListener(() => { currentSpeed = maxWalkSpeed; speedUp = false; });

        //controlUI.endurance.Setup(player.stats.Endurance);
    }



	public void OpenRadialMenu()
    {
		GeneralAvailability.Player.Lock();
		controlUI.buttonCloseRadialMenu.OpenWindow();
		controlUI.radialMenu.OpenRadialMenu();
	}
	public void CloseRadialMenu()
    {
		GeneralAvailability.Player.UnLock();
		controlUI.radialMenu.CloseRadialMenu();
		controlUI.buttonCloseRadialMenu.CloseWindow();
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

		CloseRadialMenu();
	}
}