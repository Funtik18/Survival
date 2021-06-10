using System.Collections;

public abstract class EffectBase<T> : Effect where T : EffectSD
{
	public T Data { get; protected set; }

	protected Player player;

	public EffectBase(T data)
	{
		Data = data;
	}

	public override void Execute(Player player)
	{
		this.player = player;
		player.StartCoroutine(Execution());
	}

	protected abstract IEnumerator Execution();
}