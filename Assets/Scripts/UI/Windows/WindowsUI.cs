using UnityEngine;

public class WindowsUI : MonoBehaviour
{
	public WindowBuild buildingWindow;
	public WindowItemInspector itemInspectorWindow;
	public WindowBackpack backpackWindow;

	[Space]
	public WindowResting restingWindow;
	
	[Space]
	public WindowIgnition ignitionWindow;
	public WindowFireMenu fireMenuWindow;
	[Space]
	public WindowHarvesting harvestingWindow;
	public WindowHarvestingCarcass harvestingCarcassWindow;
}