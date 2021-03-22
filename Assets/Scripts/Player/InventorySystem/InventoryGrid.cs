using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

using Sirenix.OdinInspector;

public class InventoryGrid : MonoBehaviour
{
	[SerializeField] private ScrollRect scrollrect;
	[SerializeField] private Scrollbar scrollbar;
	[AssetList]
	[SerializeField] private InventorySlot slotPrefab;


	[HideInInspector] public List<InventorySlot> slots = new List<InventorySlot>();

	private void Awake()
	{
		UpdateScrollBarSteps();
	}

	private void UpdateScrollBarSteps()
	{
		int rows = (transform.childCount / 5);
		scrollbar.numberOfSteps = (rows - 4) + 1;
	}


	[Button]
	private void AddRow()
	{
		for(int i = 0; i < 5; i++)
		{
			Instantiate(slotPrefab, transform);
		}

		int rows = (transform.childCount / 5);
		scrollbar.numberOfSteps = (rows - 4) + 1;
	}
	[Button]
	private void RemoveRow()
	{
		Debug.LogError(transform.childCount);

		List<GameObject> row = new List<GameObject>();

		for(int i = 0; i < 5; i++)
		{
			GameObject go = transform.GetChild(transform.childCount - 1).gameObject;
			go.transform.SetParent(null);
			row.Add(go);
		}

		for(int i = 0; i < row.Count; i++)
		{
			Destroy(row[i]);
		}


		Debug.LogError(transform.childCount);
		UpdateScrollBarSteps();
	}
}