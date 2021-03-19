public class Modifier
{
	public float value;

	public Modifier(float value)
	{
		this.value = value;
	}
}

public interface IModifiable
{
	void AddModifier(Modifier modifier);
	void RemoveModifier(Modifier modifier);
}