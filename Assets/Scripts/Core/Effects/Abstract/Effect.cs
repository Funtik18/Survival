using System.Collections;

using UnityEngine;
using UnityEngine.Events;

public abstract class Effect
{
	public UnityAction<Effect> onStart;
	public UnityAction<Effect> onUpdate;
	public UnityAction<Effect> onEnd;

	public bool IsEffectProcessing => coroutine != null;
	private Coroutine coroutine;

	public virtual Effect Setup(UnityAction<Effect> start = null, UnityAction<Effect> update = null, UnityAction<Effect> end = null)
	{
		onStart = start;
		onUpdate = update;
		onEnd = end;
		return this;
	}

	public abstract void Execute(Player player);

	[System.Serializable]
    public class EffectAction 
	{
		[Min(1)]
		[Tooltip("Количество выполнений.")]
		public float actionAmount = 10f;
		[Min(0.1f)]
		[Tooltip("Шаг выполнения (Формула : actionCurrentStep += actionStep).")]
		public float actionStep = 1f;
		[Min(0.1f)]
		[Tooltip("Задержка между выполнениями.")]
		public float actionDelay = 0.25f;
	}
}
