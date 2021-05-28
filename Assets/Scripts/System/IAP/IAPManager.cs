using UnityEngine;

public class IAPManager : MonoBehaviour
{
	private static IAPManager instance;
	public static IAPManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = FindObjectOfType<IAPManager>();
			}
			return instance;
		}
	}

	private string removeADS = "";
}