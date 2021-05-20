using Sirenix.OdinInspector;

using System;

public static class DataHolder
{
	private static Data data;
	public static Data Data 
	{
		get
		{
			if (data == null)
            {
				data = new Data();
			}

			return data;
		}
        set
        {
			data = value;
        }
	}

	public static LoadType loadType = LoadType.DEBUG;

	public static void Save()
    {
		SaveLoadManager.SaveData(Data);
    }
}
public class Data
{
	public JsonDateTime date;

	public Times time;

	public Statistic statistic;

	public PlayerData playerData;
	public WeatherController.WeatherForecast weatherForecast;

	public Difficult difficult;

	public Data()
    {
		playerData = new PlayerData();
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