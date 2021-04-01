using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonfireBuilding : BuildingObject
{
    [SerializeField] private bool isEnableOnAwake = false;

    [SerializeField] private List<ParticleSystem> particles = new List<ParticleSystem>();
    [Space]
    [SerializeField] private Light bonfireLight;
    [Range(0f, 8f)]
    [SerializeField] private float minIntensity = 1.5f;
    [Range(0f, 8f)]
    [SerializeField] private float maxIntensity = 2.5f;

    private bool isEnable = false;
    private float randomValue;

    protected override void Awake()
    {
        base.Awake();
        if (isEnableOnAwake)
            EnableParticles();
    }

    private void Update()
    {
        if (isEnable)
        {
            bonfireLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, Mathf.PerlinNoise(randomValue, Time.time));
        }
    }

    public void EnableParticles()
    {
        randomValue = Random.Range(0.0f, 65000f);

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

    public override void StartObserve()
    {
        Button.pointer.AddPressListener(EnableDisable);
        base.StartObserve();

    }
    public override void EndObserve()
    {
        base.EndObserve();
        Button.pointer.RemovePressListener(EnableDisable);
    }

    private void EnableDisable()
    {
        if (isEnable)
            DisableParticles();
        else
            EnableParticles();
    }
}
