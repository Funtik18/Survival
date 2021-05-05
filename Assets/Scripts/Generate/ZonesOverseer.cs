using Sirenix.OdinInspector;

using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class ZonesOverseer : MonoBehaviour
{
    [SerializeField] private List<Generator> zones = new List<Generator>();

    private Transform player;

    private WaitForSeconds seconds;


    private void Awake()
    {
        player = GeneralAvailability.Player.transform;

        seconds = new WaitForSeconds(1);

        StartCoroutine(See());
    }

    [Button]
    private void UpdateZones()
    {
        zones.Clear();

        zones = transform.GetComponentsInChildren<Generator>().ToList();
    }

    private IEnumerator See()
    {
        while (true)
        {
            for (int i = 0; i < zones.Count; i++)
            {
                Generator generator = zones[i];

                if (generator.IsEmpty)
                {
                    if (Vector3.Distance(player.position, generator.transform.position) <= generator.zoneRadius)
                    {
                        generator.ReGenerateZone();
                    }
                }
            }

            yield return seconds;
        }
    }
}
