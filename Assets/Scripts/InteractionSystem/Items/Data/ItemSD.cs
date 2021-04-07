using UnityEngine.Events;
using UnityEngine;

using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "Inventory/Item", fileName = "Item")]
public class ItemSD : ObjectScriptableData
{
	[PreviewField]
	public Sprite itemSprite;

	[AssetList]
	[InlineEditor(InlineEditorModes.GUIAndPreview)]
	public ItemObject model;

	[Range(0.01f, 99.99f)]
	public float weight = 0.01f;

	public bool isBreakable = true;

	[Min(1)]
	public int stackSize = 1;
}

[System.Serializable]
public class ItemData
{
	public UnityAction onDataChanged;

	[Required]
	public ItemSD scriptableData;

	[MaxValue("MaxStackSize")]
	[Min(1)]
	[SerializeField] private int currentStackSize = 1;
	public int CurrentStackSize
    {
		get => currentStackSize;
        set
        {
			currentStackSize = value;
			onDataChanged?.Invoke();
        }
    }

	[MinValue("Durrability")]
	[Range(0f, 100f)]
	[SerializeField] private float currentDurrability = 100f;
	public float CurrentDurrability
    {
		get => currentDurrability;
        set
        {
			currentDurrability = value;
			onDataChanged?.Invoke();
        }
    }

	private float MaxStackSize 
	{
        get
        {
			if(scriptableData != null)
            {
				return scriptableData.stackSize;
			}
			return 1;
		}
	}
	private float Durrability
	{
		get 
		{
			if(scriptableData != null)
            {
				return scriptableData.isBreakable ? 0 : 100f;
			}
			return 100f;
		}
	}
}

public class Item
{
	public System.Guid ID { get; protected set; }

	public ItemData itemData;

	public Item(ItemData itemData)
    {
		ID = System.Guid.NewGuid();

		this.itemData = itemData;
	}
}