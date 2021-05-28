using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.Events;

public class ItemObject : WorldObject
{
	public UnityAction onDisable;

	[SerializeField] protected ItemDataWrapper itemData;
	public ItemDataWrapper Data => itemData;
	[ShowIf("CheckCan")]
	public ItemObjectLiquidContainer canItemObject;
	[ShowIf("CheckCan")]
	[SerializeField] private GameObject baseCan;
	[ShowIf("CheckCan")]
	[SerializeField] private GameObject opennedCan;

	private Item item;
	public Item Item
    {
        get
        {
			if(item == null)
            {
				item = new Item(itemData);
			}
			return item;
		}
    }

	public virtual void UpdateItem() { }
	public virtual void UpdateItem(float temperature) { }


	public override void StartObserve()
	{
		base.StartObserve();

		InteractionButton.pointer.AddPressListener(Interact);
		InteractionButton.SetIconOnPickUp();
		InteractionButton.OpenButton();
		GeneralAvailability.TargetPoint.SetToolTipText(itemData.scriptableData.objectName).ShowToolTip();
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
		GeneralAvailability.Inspector.SetItem(this);
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
		itemData.scriptableData.orientation.position = transform.localPosition;
    }
	private void SaveRotation()
    {
		itemData.scriptableData.orientation.rotation = transform.localRotation;
	}
    #endregion

    private bool CheckCan() => itemData.scriptableData is CannedFoodItemSD;


    protected virtual void OnDisable()
    {
		onDisable?.Invoke();

		onDisable = null;
	}
}