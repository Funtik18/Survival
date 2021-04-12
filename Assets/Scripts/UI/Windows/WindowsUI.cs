using UnityEngine;

using Sirenix.OdinInspector;

public class WindowsUI : MonoBehaviour
{
	public WindowBuild buildingWindow;
	public WindowItemInspector itemInspectorWindow;
	public WindowBackpack backpackWindow;

	[Space]
	public WindowExchanger exchangerWindow;
	public WindowResting restingWindow;
	
	[Space]
	public WindowIgnition ignitionWindow;
	public WindowHarvesting harvestingWindow;

	[Button]
	private void OpenInspector()
    {
		itemInspectorWindow.ShowWindow();
	}
	[Button]
	private void CloseInspector()
    {
		itemInspectorWindow.HideWindow();
	}

	[Button]
	private void OpenItemInspector()
	{
		backpackWindow.OpenItemInspector();
		backpackWindow.ShowWindow();
	}
	[Button]
	private void OpenSecondaryContainer()
	{
		backpackWindow.OpenSecondaryContainer();
		backpackWindow.ShowWindow();
	}
	[Button]
	private void CloseInventory()
    {
		backpackWindow.HideBackpack();
	}
}