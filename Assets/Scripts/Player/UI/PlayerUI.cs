using Sirenix.OdinInspector;

using UnityEngine;

public class PlayerUI : MonoBehaviour
{
	//states
	public EnduranceUI endurance;

	[Title("Control")]
	public PlayerControlUI controlUI;
	

	[Title("Windows")]
	public WindowItemInspector itemInspector;

	public void Setup(Player player)
	{
		endurance.Setup(player.stats.Endurance);
	}
}
