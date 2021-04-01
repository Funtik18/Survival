using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonfireBuilding : BuildingObject
{
    [SerializeField] private List<ParticleSystem> particles = new List<ParticleSystem>();
    [Space]
    [SerializeField] private Light bonfireLight;
    [Range(0f, 8f)]
    [SerializeField] private float minIntensity = 1.5f;
    [Range(0f, 8f)]
    [SerializeField] private float maxIntensity = 2.5f;

    private bool isEnable = false;
    private float randomValue;

    private void Update()
    {
        if (isEnable)
        {
            randomValue = Random.Range(0.0f, 65000f);
            bonfireLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, Mathf.PerlinNoise(randomValue, Time.time));
        }
    }

    public void EnableParticles()
    {
        for (int i = 0; i < particles.Count; i++)
        {
            particles[i].Play();
        }
        isEnable = true;

        bonfireLight.enabled = true;
    }
    public void DisableParticles()
    {
        bonfireLight.enabled = false;
        isEnable = false;
        for (int i = 0; i < particles.Count; i++)
        {
            particles[i].Stop();
        }
    }
}
