using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.Events;

public class ItemObject : WorldObject
{
	public UnityAction onDisable;

	[SerializeField] private ItemDataWrapper data;
	public Item Item { get; private set; }

	[ShowIf("IsCan")]
	public ItemObjectLiquidContainer canItemObject;
	[ShowIf("IsCan")]
	[SerializeField] private GameObject baseCan;
	[ShowIf("IsCan")]
	[SerializeField] private GameObject opennedCan;

	private bool IsCan => data.IsCanFood;

    public virtual void UpdateItem() { }
	public virtual void UpdateItem(float temperature) { }


	public override void StartObserve()
	{
		base.StartObserve();

		InteractionButton.pointer.AddPressListener(Interact);
		InteractionButton.SetIconOnPickUp();
		InteractionButton.OpenButton();

		if(Item == null)
        {
			GeneralAvailability.TargetPoint.SetToolTipText(data.scriptableData.objectName).ShowToolTip();
        }
        else
        {
			GeneralAvailability.TargetPoint.SetToolTipText(Item.itemData.scriptableData.objectName).ShowToolTip();
		}
	}
    public override void EndObserve()
    {
        base.EndObserve();
		GeneralAvailability.TargetPoint.HideToolTip();
		InteractionButton.CloseButton();
		InteractionButton.pointer.RemovePressListener(Interact);
	}
	public override void Interact()
	{
		if (Item == null)
			SetItem(data.GetData());//генерация айтема

		GeneralAvailability.Inspector.SetItem(this);

		Overseer.Instance.Subscribe(this);
	}

    #region POs
    [Button]
	private void SaveGlobalOrientation()
    {
		SavePosition();
		SaveRotation();
	}
	private void SavePosition()
    {
		Item.itemData.scriptableData.orientation.position = transform.localPosition;
    }
	private void SaveRotation()
    {
		Item.itemData.scriptableData.orientation.rotation = transform.localRotation;
	}
    #endregion

    protected virtual void OnDisable()
    {
		onDisable?.Invoke();

		onDisable = null;
	}

	public void SetItem(ItemDataWrapper itemData)
    {
		if(Item == null)
        {
			Item = new Item(itemData);
        }
        else
        {
			Item.itemData = itemData;
		}
	}

	public void SetData(Data data)
    {
		gameObject.SetActive(data.stay.isEnable);
		transform.position = data.stay.position;
		transform.rotation = data.stay.rotation;

		SetItem(data.itemData);
	}
	public Data GetData()
    {
		Data data = new Data()
		{
			index = Overseer.Instance.IndexOfItem(this),
			stay = new Stay3()
			{
				isEnable = gameObject.activeSelf,
				position = transform.position,
				rotation = transform.rotation,
			},
			itemData = Item.itemData,
		};

		return data;
	}


	[System.Serializable]
	public class Data 
	{
		public int index;
		public Stay3 stay;
		public ItemDataWrapper itemData;
	}
}