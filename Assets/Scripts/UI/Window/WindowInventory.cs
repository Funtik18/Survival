using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WindowInventory : WindowUI
{
	public UnityAction onBack;

	[SerializeField] private Button buttonBack;

	private void Awake()
	{
		buttonBack.onClick.AddListener(() => { onBack?.Invoke(); });
	}
}