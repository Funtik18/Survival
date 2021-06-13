using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.Events;

public class ItemObject : WorldObject
{
	public UnityAction onDisable;

	[SerializeField] private ItemDataWrapper data;
	public ItemDataWrapper CurrentData { get; set; }

	[ShowIf("IsCan")]
	public ItemObjectLiquidContainer canItemObject;
	[ShowIf("IsCan")]
	[SerializeField] private GameObject baseCan;
	[ShowIf("IsCan")]
	[SerializeField] private GameObject opennedCan;

	private bool IsCan => CurrentData.IsCanFood;

    public virtual void UpdateItem() { }
	public virtual void UpdateItem(float temperature) { }
	public virtual void ItemAction() { }


	public override void StartObserve()
	{
		base.StartObserve();

		InteractionButton.pointer.AddPressListener(Interact);
		InteractionButton.SetIconOnPickUp();
		InteractionButton.OpenButton();

		GeneralAvailability.TargetPoint.SetToolTipText(data.scriptableData.objectName).ShowToolTip();
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
		TryGenerateItemData();

		GeneralAvailability.Player.Inspector.SetItem(this);
		GeneralAvailability.PlayerUI.OpenItemInspector(CurrentData);

		Overseer.Instance.Subscribe(this);
	}
	/// <summary>
	/// Взаимодействия, когда сразу создаём объект и сразу его проверяем.
	/// </summary>
	public void InteractSub()
    {
		TryGenerateItemData();
		GeneralAvailability.PlayerUI.OpenItemInspector(CurrentData);
	}
	private ItemObject TryGenerateItemData()
	{
		if (CurrentData == null)
			CurrentData = data.GetData();//генерация айтема

		return this;
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
		CurrentData.scriptableData.orientation.position = transform.localPosition;
    }
	private void SaveRotation()
    {
		CurrentData.scriptableData.orientation.rotation = transform.localRotation;
	}
    #endregion

	public void SetData(Data data)
    {
		gameObject.SetActive(data.stay.isEnable);
		transform.position = data.stay.position;
		transform.rotation = data.stay.rotation;

		CurrentData = data.itemData;
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
			itemData = CurrentData,
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

	protected virtual void OnDisable()
	{
		onDisable?.Invoke();

		onDisable = null;
	}
}