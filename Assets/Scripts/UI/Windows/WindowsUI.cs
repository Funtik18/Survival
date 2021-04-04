using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

public class WindowsUI : MonoBehaviour
{
	public WindowBuild buildingWindow;
	public WindowItemInspector itemInspectorWindow;
	public WindowBackpack backpackWindow;
	public WindowExchanger exchangerWindow;

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