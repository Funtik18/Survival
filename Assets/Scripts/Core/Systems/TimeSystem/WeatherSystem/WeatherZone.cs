using Sirenix.OdinInspector;

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class WeatherZone : MonoBehaviour
{
    public UnityAction onTemperatureChanged;

    public AnimationCurve temperatureDistanceCurve;

    [SerializeField] private float temperatureInZone;
    public float TemperatureInZone 
    {
        get => temperatureInZone;
        set
        {
            temperatureInZone = value;

            onTemperatureChanged?.Invoke();
        }
    }

    [OnValueChanged("UpdateSize")]
    [SerializeField] private Vector3 minSize;
    [OnValueChanged("UpdateSize")]
    [SerializeField] private Vector3 maxSize;

    public Vector3 CurrentSize => Transform.localScale;

    [Space]
    [Range(0, 1f)]
    [OnValueChanged("UpdateSize")]
    [SerializeField] private float size;

    private Transform trans;
    protected Transform Transform
    {
        get
        {
            if (trans == null)
                trans = transform;
            return trans;
        }
    }

    private Collider coll;
    private Collider Collider
    {
        get
        {
            if(coll == null)
            {
                coll = GetComponent<Collider>();
            }
            return coll;
        }
    }

    [SerializeField] private bool isEnable = false;
    public bool IsEnable
    {
        get => isEnable;
        set
        {
            isEnable = value;

            if (isEnable == false)
            {
                collidersIntersects.Clear();
                Collider.enabled = false;
            }
            else
            {
                if (!Collider.enabled)
                    Collider.enabled = true;
            }
        }
    }

    public float Radius => Collider.bounds.extents.magnitude;
    public float Diameter => Radius/2;

    [SerializeField] protected LayerMask intersectLayers;

    [SerializeField] protected List<Collider> collidersIntersects = new List<Collider>();

    private void Awake()
    {
        IsEnable = isEnable;
    }

    public void SetSize(float normalValue)
    {
        size = normalValue;

        UpdateSize();
    }

    private void UpdateSize()
    {
        Transform.localScale = Vector3.Lerp(minSize, maxSize, size);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsEnable) return;

        if (!collidersIntersects.Contains(other))
        {
            if (((1 << other.gameObject.layer) & intersectLayers) != 0)
            {
                collidersIntersects.Add(other);

                Player p =  other.GetComponent<Player>();
                if (p)
                {
                    p.Status.resistances.AddZone(this);
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (!IsEnable) return;

        if (collidersIntersects.Contains(other))
        {
            collidersIntersects.Remove(other);

            Player p = other.GetComponent<Player>();
            if (p)
            {
                p.Status.resistances.RemoveZone(this);
            }
        }
    }
}