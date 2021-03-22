public class ItemModel : PickableItem 
{
	public ItemInspectorAngle itemAngle = ItemInspectorAngle.Identity;

	public ItemScriptableData scriptableData;
}
public enum ItemInspectorAngle
{
	Identity,
	World,
}