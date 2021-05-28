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

    [SerializeField] private NewGamePattern newGamePattern;

    private Coroutine saveCoroutine = null;
    public bool IsSaveProccess => saveCoroutine != null;

    private void Awake()
    {
        if (DataHolder.loadType == LoadType.DEBUG)
        {
            GeneralAvailability.Player.IsDEBUG();

            if (WeatherController.Instance.CurrentForecast == null) { }

            Debug.LogError("DEBUG");
        }
        else
        {
            Debug.LogError("LOAD GAME");

            if (DataHolder.loadType == LoadType.NewGame)
            {
                Data data = newGamePattern.GetData();

                GeneralTime.Instance.SetTime(data.time.TotalSeconds);
                GeneralAvailability.Player.SetData(data.playerData);

                GeneralStatistics.Init();
            }
            else if (DataHolder.loadType == LoadType.Continue)
            {
                Data data = DataHolder.Data;

                GeneralTime.Instance.SetTime(data.time.TotalSeconds);

                GeneralAvailability.Player.SetData(data.playerData);

                WeatherController.Instance.SetForecast(data.weatherForecast);

                GeneralStatistics.SetData(data.statistic);
                
                GeneralStatistics.Init();
            }
            else if (DataHolder.loadType == LoadType.Load)
            {
                Data data = DataHolder.Data;
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
        Data data = DataHolder.Data;
        data.date = System.DateTime.Now;
        data.time = GeneralTime.Instance.globalTime;
        data.statistic = GeneralStatistics.GetData();
        data.playerData = GeneralAvailability.Player.GetData();
        data.weatherForecast = WeatherController.Instance.CurrentForecast;

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

        public Data GetData()
        {
            Data data = new Data();

            data.time = randomTime ? Times.GetRandomTimes() : startTime;

            Transform point = randomPoints ? startPoints.GetRandomItem() : startPoint;

            data.playerData.position = point.position;
            data.playerData.rotation = Quaternion.LookRotation(point.forward);

            data.playerData.statusData = randomPlayerData ? playerRandomData.GetData() : playerData.statsData;

            data.playerData.inventoryData = ItemsData.Instance.PlayerContainer;

            data.weatherForecast = WeatherController.Instance.CurrentForecast;

            return data;
        }
    }
}