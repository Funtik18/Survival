using Sirenix.OdinInspector;

using System;
using System.Collections.Generic;

using UnityEngine;

public static class DataHolder
{
	private static Data currentData;
	public static Data CurrentData 
	{
		get
		{
			if (currentData == null)
            {
				currentData = new Data();
			}

			return currentData;
		}
        set
        {
			currentData = value;
        }
	}

	public static LoadType loadType = LoadType.DEBUG;

	public static void Save()
    {
		SaveLoadManager.SaveData(CurrentData);
    }

	[System.Serializable]
	public class NewGamePattern
	{
		public bool randomTime = true;
		[HideIf("randomTime")]
		public Times startTime;

		public bool randomPoints = false;
		[HideIf("randomPoints")]
		public Transform startPoint;
		[ShowIf("randomPoints")]
		public List<Transform> startPoints = new List<Transform>();

		public bool randomPlayerData = false;

		[ShowIf("randomPlayerData")]
		public PlayerStatusRandomSD playerRandomData;

		[HideIf("randomPlayerData")]
		public PlayerStatusSD playerData;

		public DataHolder.Data GetData()
		{
			DataHolder.Data data = new DataHolder.Data();

			data.time = randomTime ? Times.GetRandomTimes() : startTime;

			Transform point = randomPoints ? startPoints.GetRandomItem() : startPoint;

			data.player.stay = new Stay3()
			{
				position = point.position,
				rotation = Quaternion.LookRotation(point.forward),
			};

			data.player.statusData = randomPlayerData ? playerRandomData.GetData() : playerData.statusData;

			data.player.inventoryData = ItemsData.Instance.PlayerContainer;

			data.weatherForecast = WeatherController.Instance.CurrentForecast;

			return data;
		}
	}


	[System.Serializable]
	public class Data
	{
		public JsonDateTime date;

		public Times time;

		public Difficult difficult;
		public Statistics.Data statistic;

		public Player.Data player;

		public Overseer.Data environment;

		public WeatherController.WeatherForecast weatherForecast;
	}
}

[System.Serializable]
public struct JsonDateTime
{
	public long value;
	public static implicit operator DateTime(JsonDateTime jdt)
	{
		return DateTime.FromFileTimeUtc(jdt.value);
	}
	public static implicit operator JsonDateTime(DateTime dt)
	{
		JsonDateTime jdt = new JsonDateTime();
		jdt.value = dt.ToFileTimeUtc();
		return jdt;
	}
}


public enum LoadType
{
	DEBUG,
	NewGame,
	Continue,
	Load,
}

public enum Gender
{
	Male,
	Female
}
public enum Difficult
{
	Normal
}