using UnityEngine.Events;
using UnityEngine;

using Sirenix.OdinInspector;

/// <summary>
/// Все текущие состояния игрока.
/// </summary>
[System.Serializable]
public class PlayerStates
{
	public UnityAction<PlayerState> onStateChanged;
	[ReadOnly]
	[SerializeField] private PlayerState currentState = PlayerState.Standing;
	public PlayerState CurrentState
	{
		get => currentState;
		set
		{
			currentState = value;
			onStateChanged?.Invoke(currentState);
		}
	}

	public void AddAction(UnityAction<PlayerState> action)
    {
		onStateChanged += action;

	}
	public void RemoveAction(UnityAction<PlayerState> action)
    {
		onStateChanged -= action;
	}
	public void RemoveAllActions()
    {
		onStateChanged = null;
    }
}
public enum PlayerState
{
	Sleeping,
	Resting,
	Standing,
	Walking,
	Sprinting,
	Climbing,
}