using Sirenix.OdinInspector;

using UnityEngine;

[CreateAssetMenu(menuName = "Game/Player/PlayerStatus", fileName = "Data")]
public class PlayerStatusSD : ScriptableObject
{
	[TabGroup("PlayerStatus")]
	[HideLabel]
	public PlayerStatus.Data statusData;
}