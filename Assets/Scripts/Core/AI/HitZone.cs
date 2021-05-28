using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HitZone : MonoBehaviour
{
    [SerializeField] private AI ai;
    [SerializeField] private Zone zone;
    private Collider collider;
    public Collider Collider
    {
        get
        {
            if (collider == null)
            {
                collider = GetComponent<Collider>();
            }
            return collider;
        }
    }

    public void Hit()
    {
        ai.Hit(zone);
    }

    public enum Zone
    {
        Head,
        Neck,
        Chest,
        Hind,
        Foot,
    }
}