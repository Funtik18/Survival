using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

public class FireBuilding : BuildingObject
{
    [PropertyOrder(-1)]
    [SerializeField] protected bool isEnableOnAwake = false;
    [SerializeField] protected List<ParticleSystem> particles = new List<ParticleSystem>();
    [Space]
    [SerializeField] protected Light bonfireLight;
    [Range(0f, 8f)]
    [SerializeField] protected float minIntensity = 1.5f;
    [Range(0f, 8f)]
    [SerializeField] protected float maxIntensity = 2.5f;

    protected bool isEnable = false;
    protected float randomValue;

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


    public override void StartObserve()
    {
        base.StartObserve();
        View();
    }
    public override void EndObserve()
    {
        base.EndObserve();
        InteractionButton.pointer.RemoveAllPressListeners();
    }

    protected virtual void View()
    {
        if (!isEnable)
            InteractionButton.pointer.AddPressListener(OpenIgnitionWindow);
    }


    protected void OpenIgnitionWindow()
    {
        GeneralAvailability.PlayerUI.OpenIgnition(this);
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

    private void EnableDisable()
    {
        if (isEnable)
            DisableParticles();
        else
            EnableParticles();
    }
}
