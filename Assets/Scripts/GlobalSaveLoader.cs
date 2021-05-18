using Sirenix.OdinInspector;

using System.Collections.Generic;

using UnityEngine;

public class GlobalSaveLoader : MonoBehaviour
{
    [SerializeField] private NewGamePattern newGamePattern;

    private void Awake()
    {
        if (DataHolder.loadType == LoadType.DEBUG)
        {
            GeneralAvailability.Player.IsDEBUG();
            Debug.LogError("DEBUG");
        }
        else
        {
            Debug.LogError("LOAD GAME");

            if (DataHolder.loadType == LoadType.NewGame)
            {
                Data data = newGamePattern.GetData();
                GeneralAvailability.Player.SetData(data.playerData);
            }
            else if (DataHolder.loadType == LoadType.Continue)
            {
                Data data = DataHolder.Data;

                GeneralAvailability.Player.SetData(data.playerData);
            }
            else if (DataHolder.loadType == LoadType.Load)
            {
                Data data = DataHolder.Data;
            }
        }
    }


    [Button]
    private void Save()
    {
        Data data = DataHolder.Data;
        data.date = System.DateTime.Now;
        data.playerData = GeneralAvailability.Player.GetData();

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

        public bool randomInventory = false;

        [HideIf("randomInventory")]
        public ContainerSD container;

        public Data GetData()
        {
            Data data = new Data();

            Transform point;

            if (randomPoints)
            {
                point = startPoints.GetRandomItem();
            }
            else
            {
                point = startPoint;
            }

            data.playerData.position = point.position;
            data.playerData.rotation = Quaternion.LookRotation(point.forward);

            data.playerData.statusData = randomPlayerData ? playerRandomData.GetData() : playerData.statsData;

            if (randomInventory)
            {
                data.playerData.inventoryData = ItemsData.Instance.PlayerContainer;
            }
            else
            {
                //data.playerData.inventoryData = container.container.GetInventoryData();
            }


            return data;
        }
    }
}