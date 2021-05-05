using Sirenix.OdinInspector;

using UnityEngine;

public class ItemObject : WorldObject
{
	[SerializeField] private ItemDataWrapper itemData;
	[HideInInspector] public Item item;

	private Rigidbody rigidbody;

	private void Awake()
    {
		SetData(itemData);
	}

    private void OnCollisionEnter(Collision collision)
    {
		Debug.LogError("HERE");
	}
	private void OnTriggerEnter(Collider other)
    {
		Debug.LogError("HERE");
    }

    public void SetData(ItemDataWrapper data)
    {
		if (item == null)
		{
			item = new Item(data);
		}
        else
        {
			item.itemData = data;
        }
	}

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
}