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

    [SerializeField] private List<CookingSlot> slots = new List<CookingSlot>();

    private Times fireStart;
    private Times fireDuration;
    private Times fireEnd;

    private Times current;

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
        if (isEnableOnAwake)
        {
            UpdateItem();
        }
    }

    public bool AddItem(Item item)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].IsEmpty)
            {
                slots[i].SetItem(item);
                return true;
            }
        }

        return false;
    }

    private void UpdateItem()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].UpdateItem();
        }
    }

    #region Fire
    public void StartFire(Times fireDuration)
    {
        if (!isEnable)
        {
            isEnable = true;

            this.fireStart = GeneralTime.Instance.globalTime;
            this.fireDuration = fireDuration;
            this.fireEnd = fireStart + fireDuration;


            GeneralTime.TimeUnityEvent unityEvent = new GeneralTime.TimeUnityEvent();
            unityEvent.AddEvent(GeneralTime.TimeUnityEvent.EventType.ExecuteInTime, fireEnd, null, UpdateFire, StopFire);
            GeneralTime.Instance.AddEvent(unityEvent);

            EnableParticles();
        }
    }
    public void UpdateFire(Times time)
    {
        current = fireEnd - time;

        UpdateItem();
    }
    public void StopFire()
    {
        if (isEnable)
        {
            DisableParticles();
            isEnable = false;
        }
    }
    #endregion

    #region Observe
    public override void StartObserve()
    {
        base.StartObserve();
        if (isEnable)
        {
            if(isEnableOnAwake)
                GeneralAvailability.TargetPoint.SetTooltipAddText(current.ToStringSimplification(isInfinity : true)).ShowAddToolTip();
            else
                GeneralAvailability.TargetPoint.SetTooltipAddText(current.ToStringSimplification(true)).ShowAddToolTip();
        }
        View();
    }
    public override void Observe()
    {
        if (isEnable)
        {
            base.Observe();
            
            if(!isEnableOnAwake)
                GeneralAvailability.TargetPoint.SetTooltipAddText(current.ToStringSimplification(true));
        }
    }
    public override void EndObserve()
    {
        base.EndObserve();
        GeneralAvailability.TargetPoint.HideAddToolTip();
        InteractionButton.pointer.RemoveAllPressListeners();
    }

    protected virtual void View()
    {
        if (isEnable)
            InteractionButton.pointer.AddPressListener(OpenCookingWindow);
        else
            InteractionButton.pointer.AddPressListener(OpenIgnitionWindow);
    }

    protected void OpenIgnitionWindow()
    {
        GeneralAvailability.PlayerUI.OpenIgnition(this);
    }
    private void OpenCookingWindow()
    {
        GeneralAvailability.PlayerUI.OpenFireMenu(this);
    }
    #endregion


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
