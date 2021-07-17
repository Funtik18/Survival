using Sirenix.OdinInspector;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    [SerializeField] private bool isCanReGenerate = false;
    [ShowIf("isCanReGenerate")]
    [SerializeField] private Times howOften;
    [Space]
    [SerializeField] private List<GameObject> prefabs = new List<GameObject>();

    [Min(1)]
    public float prefabRadius;

    public float zoneRadiusOffset;

    [SerializeField] private GeneratorType type = GeneratorType.Sphere;

    [SerializeField] private Vector2Int countPrefabs;
    [Space]
    [SerializeField] private float yOffset = 0.0f;

    [HideInInspector] public List<Transform> objs = new List<Transform>();

    [SerializeField] private float zoneRadiusAwake = 0;

    public bool IsCanGenerate => !isGenerated && objs.Count == 0;
    public float ZoneRadius => Mathf.Max(transform.localScale.x / 2, transform.localScale.y / 2, transform.localScale.z / 2);

    private Transform trans;
    public Transform Transform 
    {
        get
        {
            if(trans == null)
            {
                trans = transform;
            }
            return trans;
        }
    }

    private bool isGenerated = false;
    private bool isNeedRegenerate = false;

    private void Awake()
    {
        if(objs.Count != 0)
            objs.Clear();

        zoneRadiusAwake = ZoneRadius * Mathf.Sqrt(2) + zoneRadiusOffset;

        //if (isCanReGenerate)
        //{
        //    GeneralTime.TimeUnityEvent unityEvent = new GeneralTime.TimeUnityEvent();
        //    unityEvent.AddEvent(GeneralTime.TimeUnityEvent.EventType.ExecuteEveryTime, howOften, TriggerGenerate);
        //    GeneralTime.Instance.AddEvent(unityEvent);
        //}
    }

    public void Behavior(Vector3 playerPosition)
    {
        if (IsCanGenerate)
        {
            //if (Vector3.Distance(playerPosition, Transform.position) <= zoneRadiusAwake)
            //{
            //    ReGenerateZone();
            //}
        }
        else
        {
            if (isNeedRegenerate)
            {
                if(Vector3.Distance(playerPosition, Transform.position) <= zoneRadiusAwake)
                {
                    ReGenerateZone();
                    isNeedRegenerate = false;
                }
            }
        }
    }


    public void ReGenerateZone()
    {
        for (int i = 0; i < objs.Count; i++)
        {
            ObjectPool.ReturnGameObject(objs[i].gameObject);
        }

        objs.Clear();

        if(type == GeneratorType.Sphere)
        {
            GenerateSphereZone();
        }
        else if(type == GeneratorType.Box)
        {
            GenerateCubeZone();
        }

        isGenerated = true;
    }

    private void GenerateSphereZone()
    {
        int countRnd = Random.Range(countPrefabs.x, countPrefabs.y);

        for (int i = 0; i < countRnd; i++)
        {
            Vector3 point = GetRanomPointInSphereZone();

            if(objs.Count > 0)
            {
                if (CheckNeighbors(point))
                    InstantiatePrefab(point);
            }
            else
            {
                InstantiatePrefab(point);
            }
        }
    }
    private void GenerateCubeZone()
    {
        int countRnd = Random.Range(countPrefabs.x, countPrefabs.y);

        for (int i = 0; i < countRnd; i++)
        {
            Vector3 point = GetRandomPointInCubeZone();

            if (objs.Count > 0)
            {
                if (CheckNeighbors(point))
                    InstantiatePrefab(point);
            }
            else
            {
                InstantiatePrefab(point);
            }
        }
    }

    private void TriggerGenerate()
    {
        isNeedRegenerate = true;
    }


    private bool CheckNeighbors(Vector3 point)
    {
        int count = 0;
        for (int j = 0; j < objs.Count; j++)
        {
            if (Vector3.Distance(point, objs[j].position) >= prefabRadius)
            {
                count++;
            }
        }

        return count == objs.Count;
    }

    private void InstantiatePrefab(Vector3 point)
    {
        RaycastHit hit;
        Ray ray = new Ray(point, Vector3.down);

        if(Physics.Raycast(ray, out hit))
        {
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, hit.normal);

            Transform obj = ObjectPool.GetObject(prefabs.GetRandomItem()).transform;

            obj.position = point;
            obj.rotation = rot;
            obj.Rotate(Vector3.up, Random.Range(0, 360f));

            objs.Add(obj);
        }
    }

    private Vector3 GetRanomPointInSphereZone()
    {
        Vector3 point = ExtensionRandom.GetRandomPointInCircleXZ() * ZoneRadius + transform.position;
        point.y = Terrain.activeTerrain.SampleHeight(point) + yOffset;
        return point;
    }
    private Vector3 GetRandomPointInCubeZone()
    {
        Vector3 point = ExtensionRandom.RandomPointInBox(transform.position, transform.localScale);
        point.y = Terrain.activeTerrain.SampleHeight(point) + yOffset;
        return point;
    }

    private void OnDrawGizmos()
    {
        if(type == GeneratorType.Sphere)
        {
            Gizmos.DrawWireSphere(transform.position, ZoneRadius);
        }
        else if(type == GeneratorType.Box)
        {
            Gizmos.DrawWireCube(transform.position, transform.localScale);
        }
    }

    public enum GeneratorType 
    {
        Box,
        Sphere,
    }
}