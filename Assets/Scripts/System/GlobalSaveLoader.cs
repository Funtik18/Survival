using Sirenix.OdinInspector;

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class GlobalSaveLoader : MonoBehaviour
{
    private static GlobalSaveLoader instance;
    public static GlobalSaveLoader Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<GlobalSaveLoader>();
            }
            return instance;
        }
    }

    [InfoBox("Класс выбирает тип запуска, это дебаг мод или новая игра или произошла загрузка игры.")]
    [SerializeField] private DataHolder.NewGamePattern newGamePattern;

    private Coroutine saveCoroutine = null;
    public bool IsSaveProccess => saveCoroutine != null;

    private void Awake()
    {
        if (DataHolder.loadType == LoadType.DEBUG)
        {
            Debug.LogError("DEBUG");

            GeneralAvailability.Player.SetData(null);
        }
        else
        {
            if (DataHolder.loadType == LoadType.NewGame)
            {
                Debug.LogError("NEW GAME");

                DataHolder.Data data = newGamePattern.GetData();

                GeneralTime.Instance.SetTime(data.time.TotalSeconds);
                GeneralAvailability.Player.SetData(data.player);

                Statistics.Init();
            }
            else if (DataHolder.loadType == LoadType.Continue)
            {
                Debug.LogError("CONTINUE GAME");

                DataHolder.Data data = DataHolder.CurrentData;

                GeneralTime.Instance.SetTime(data.time.TotalSeconds);

                GeneralAvailability.Player.SetData(data.player);

                Overseer.Instance.SetData(data.environment);

                WeatherController.Instance.SetForecast(data.weatherForecast);

                Statistics.SetData(data.statistic);
                Statistics.Init();
            }
            else if (DataHolder.loadType == LoadType.Load)
            {
                Debug.LogError("LOAD GAME");

                DataHolder.Data data = DataHolder.CurrentData;
            }
        }
    }


    public void StartSaveGame()
    {
        if (!IsSaveProccess)
        {
            saveCoroutine = StartCoroutine(SaveGame());
        }
    }
    private IEnumerator SaveGame()
    {
        GeneralAvailability.PlayerUI.savingPanel.Enable(true);

        yield return new WaitForSeconds(1f);
        Save();
        yield return new WaitForSeconds(4f);
        GeneralAvailability.PlayerUI.savingPanel.Enable(false);

        StopSaveGame();
    }
    private void StopSaveGame()
    {
        if (IsSaveProccess)
        {
            StopCoroutine(saveCoroutine);
            saveCoroutine = null;
        }
    }

    [Button]
    private void Save()
    {
        GeneralAvailability.Player.Status.opportunities.UnEquip();


        DataHolder.Data data = DataHolder.CurrentData;
        data.date = System.DateTime.Now;//время
        data.time = GeneralTime.Instance.globalTime;//игровое время
        data.statistic = Statistics.GetData();//статистика
        data.player = GeneralAvailability.Player.GetData();//игрок
        data.environment = Overseer.Instance.GetData();//окружение
        data.weatherForecast = WeatherController.Instance.CurrentForecast;//погода

        DataHolder.Save();
    }

    private void OnDrawGizmos()
    {
        if (newGamePattern.randomPoints)
        {
            for (int i = 0; i < newGamePattern.startPoints.Count; i++)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(newGamePattern.startPoints[i].position, 1f);
                
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(newGamePattern.startPoints[i].position, newGamePattern.startPoints[i].position + newGamePattern.startPoints[i].forward * 2f);

                Gizmos.color = Color.green;
                Gizmos.DrawLine(newGamePattern.startPoints[i].position, newGamePattern.startPoints[i].position - newGamePattern.startPoints[i].up * 3.5f);
            }
        }
    }
}