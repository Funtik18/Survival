using Sirenix.OdinInspector;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public float zoneRadius;
    [Space]
    [SerializeField] private List<GameObject> prefabs = new List<GameObject>();
    [Min(1)]
    public float prefabRadius;

    public Vector2Int countPrefabs;
    [Space]
    public float yOffset = 0.0f;

    [HideInInspector] public List<Transform> objs = new List<Transform>();

    public bool IsEmpty => objs.Count == 0;

    private ObjectPool pool;
    private ObjectPool Pool
    {
        get
        {
            if (pool == null)
            {
                pool = ObjectPool.Instance;
            }
            return pool;
        }
    }


    private void Awake()
    {
        pool = ObjectPool.Instance;
        
        if(objs.Count != 0)
            objs.Clear();
    }


    public void ReGenerateZone()
    {
        ObjectPool temp = Pool;
        for (int i = 0; i < objs.Count; i++)
        {
            temp.ReturnGameObject(objs[i].gameObject);
        }
        objs.Clear();
        GenerateZone();
    }

    [Button]
    public void GenerateZone()
    {
        int countRnd = Random.Range(countPrefabs.x, countPrefabs.y);

        for (int i = 0; i < countRnd; i++)
        {
            Vector3 point = GetRanomPointInZone();

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
        Debug.LogError(objs.Count);
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

            Transform obj = Pool.GetObject(prefabs.GetRandomItem()).transform;
            
            obj.position = point;
            obj.rotation = rot;
            obj.Rotate(Vector3.up, Random.Range(0, 360f));

            objs.Add(obj);
        }
    }

    private Vector3 GetRanomPointInZone()
    {
        Vector3 point = ExtensionRandom.GetRandomPointInCircleXZ() * zoneRadius + transform.position;
        point.y = Terrain.activeTerrain.SampleHeight(point) + yOffset;
        return point;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, zoneRadius);
    }
}