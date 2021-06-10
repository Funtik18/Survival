using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

public class SaveLoadManager
{
	private readonly static string expansion = ".txt";
	private readonly static string directorySaves = "/saves/";


	//private static int isFirstTime = -1;
	//public static bool IsFirstTime
	//{
	//	set
	//	{
	//		isFirstTime = value ? 1 : 0;
	//		PlayerPrefs.SetInt("OnFirstTime", isFirstTime);
	//	}
	//	get
	//	{
	//		if (isFirstTime == -1)
	//		{
	//			isFirstTime = PlayerPrefs.GetInt("OnFirstTime", -1);
	//			if (isFirstTime == -1)
	//			{
	//				IsFirstTime = true;
	//			}
	//		}
	//		return isFirstTime == 1 ? true : false;
	//	}
	//}

	public static List<DataHolder.Data> GetAllSaves()
    {
		List<DataHolder.Data> data = new List<DataHolder.Data>();
		string fullPath = Application.persistentDataPath + directorySaves;

		if (!Directory.Exists(fullPath))
		{
			Directory.CreateDirectory(fullPath);

			return data;
		}

		string[] files = Directory.GetFiles(fullPath);
        for (int i = 0; i < files.Length; i++)
        {
			data.Add(LoadDataFromJson<DataHolder.Data>(files[i]));
        }

		return data;
	}



	public static void SaveData(DataHolder.Data data)
    {
		SaveDataToJson(data, directorySaves, "SaveFile" + expansion);
	}



	/// <summary>
	/// Сохранение объекта в Json.
	/// </summary>
	private static void SaveDataToJson<T>(T data, string directory, string fileName)
	{
		string dir = Application.persistentDataPath + directory;

		if (!Directory.Exists(dir))
		{
			Directory.CreateDirectory(dir);
		}

		string jsonData = JsonUtility.ToJson(data, true);
		File.WriteAllText(dir + fileName, jsonData);
	}

	/// <summary>
	/// Загрузка объекта из Json в объект.
	/// </summary>
	private static T LoadDataFromJson<T>(string directory, string fileName)
	{
		string fullPath = Application.persistentDataPath + directory + fileName;

		if (File.Exists(fullPath))
		{
			string json = File.ReadAllText(fullPath);
			return JsonUtility.FromJson<T>(json);
		}
		else
		{
			Debug.LogError("File doesn't exist");
		}

		return default;
	}
	private static T LoadDataFromJson<T>(string fullPath)
	{
		if (File.Exists(fullPath))
		{
			string json = File.ReadAllText(fullPath);
			return JsonUtility.FromJson<T>(json);
		}
		else
		{
			Debug.LogError("File doesn't exist");
		}

		return default;
	}


	public static void DestroyDirectory(string directory = "/saves/")
	{
		Directory.Delete(Application.persistentDataPath + directory, true);
	}

}