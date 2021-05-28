using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

public class Construction : MonoBehaviour
{
    [SerializeField] private Accommodation accommodationOutside;
    [SerializeField] private Accommodation accommodationInside;
    [Space]
    [SerializeField] private List<PortaleConnection> connections = new List<PortaleConnection>();

    public void Awake()
    {
        for (int i = 0; i < connections.Count; i++)
        {
            connections[i].Setup();
        }
    }

    [System.Serializable]
    public class PortaleConnection
    {
        public Color color = Color.white;
        [Tooltip("Точка снаружи.")]
        public Portale outside;
        [Tooltip("Точка внутри.")]
        public Portale inside;

        public void Setup()
        {
            outside.door.onInteract = PortalTransactionIn;
            outside.door.onStartObserve = ObserveInside;

            inside.door.onInteract = PortalTransactionOut;
            inside.door.onStartObserve = ObserveOutside;
        }

        private void PortalTransactionIn()
        {
            GeneralAvailability.Player.ChangePosition(inside.Point, inside.Rotation);
        }
        private void PortalTransactionOut()
        {
            GeneralAvailability.Player.ChangePosition(outside.Point, outside.Rotation);
        }

        private void ObserveInside()
        {
            GeneralAvailability.TargetPoint.SetToolTipText(outside.Phrase);
        }
        private void ObserveOutside()
        {
            GeneralAvailability.TargetPoint.SetToolTipText(inside.Phrase);
        }


        [System.Serializable]
        public class Portale
        {
            public ConstructionSD data;
            public DoorObject door;
            public Transform pivot;
            public UnityEvent onInteract;

            public Vector3 Point => pivot.position;
            public Quaternion Rotation => pivot.rotation;

            public string Phrase => data.phrase;
        }
    }
   

    private void OnDrawGizmos()
    {
        for (int i = 0; i < connections.Count; i++)
        {
            PortaleConnection connection = connections[i];
            if (connection == null) continue;

            Gizmos.color = connection.color;

            Vector3 center = connection.inside.pivot.position;
            Vector3 forward = connection.inside.pivot.forward;

            Gizmos.DrawSphere(center, 0.2f);
            Gizmos.DrawLine(center, center + forward);

            Gizmos.DrawLine(center, connection.inside.door.transform.position);

            center = connection.outside.pivot.position;
            forward = connection.outside.pivot.forward;

            Gizmos.DrawSphere(center, 0.2f);
            Gizmos.DrawLine(center, center + forward);

            Gizmos.DrawLine(center, connection.outside.door.transform.position);
        }
    }
}