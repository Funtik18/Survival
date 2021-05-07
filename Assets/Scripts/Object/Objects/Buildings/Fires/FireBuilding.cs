using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

public class FireBuilding : BuildingObject
{
    [PropertyOrder(-1)]
    [SerializeField] protected bool isEnableOnAwake = false;
    
    [PropertyOrder(2)]
    [SerializeField] protected List<ParticleSystem> particles = new List<ParticleSystem>();
    [PropertyOrder(2)]
    [SerializeField] private List<CookingSlot> slots = new List<CookingSlot>();
    [Space]
    [SerializeField] private float maxTemperature = 80;
    [SerializeField] private Times maxFireTime = new Times() { hours = 12 };
    [Space]
    [SerializeField] protected Light bonfireLight;
    [Range(0f, 8f)]
    [SerializeField] protected float maxIntensity0 = 1.5f;
    [Range(0f, 8f)]
    [SerializeField] protected float maxIntensity1 = 2.5f;
    [Range(0f, 8f)]
    [SerializeField] protected float maxIntensity2 = 2.5f;

    [SerializeField] private WeatherZone zone;


    private WindowIgnition windowIgnition;
    private WindowIgnition WindowIgnition
    {
        get
        {
            if(windowIgnition == null)
            {
                windowIgnition = GeneralAvailability.PlayerUI.windowsUI.ignitionWindow;
            }
            return windowIgnition;
        }
    }

    private RequirementsIgnition requirementsValues;


    private string ToolTip => currentTime.ToStringSimplification(true) + " " + System.Math.Round(currentTemperature, 1) + SymbolCollector.CELSIUS;

    private float successChance;

    private float holdTime = 1f;

    private float currentTemperature;
    public float CurrentTemperature => currentTemperature;
    private float targetTemperature;

    private Times kindleTime;

    private Times fireStart;
    private Times fireDuration;
    private Times fireEnd;

    private Times currentTime;

    protected bool isEnable = false;
    protected float randomValue;


    private Coroutine holdIgnitionCoroutine = null;
    public bool IsHoldIgnitionProccess => holdIgnitionCoroutine != null;


    protected override void Awake()
    {
        base.Awake();

        if (isEnableOnAwake)
        {
            EnableParticles();
        }
    }

    //private void Update()
    //{
    //    //if (isEnable)
    //    //{
    //    //    float value = Mathf.Lerp(minIntensity, maxIntensity, Mathf.PerlinNoise(randomValue, Time.time));
    //    //    bonfireLight.intensity = value;
    //    //    warmRadius = value;
    //    //}
    //    if (isEnableOnAwake)
    //    {
    //        //UpdateItems();
    //        //UpdateEnvironment();
    //    }
    ////}

    public bool AddItem(Item item)
    {
        ItemDataWrapper data = item.itemData;
        ItemSD itemSD = data.scriptableData;
        if (itemSD is FireFuelSD fuelSD)
        {
            fireEnd += fuelSD.addFireTime;
            targetTemperature += fuelSD.addTemperature;
            unityEvent.SetTime(fireEnd);

            return true;
        }
        return false;
    }


    private void UpdateItems()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].UpdateItem();
        }
    }

    private void SetTemperature(float temperature)
    {
        currentTemperature = Mathf.Clamp(temperature, 0, targetTemperature);
    }
    private void SetFireIntensity(float value)
    {
        bonfireLight.intensity = value;
        zone.SetSize(value);
    }


    #region Fire
    #region Розжиг
    public void LightFire(float endTemperature)
    {
        if (!isEnable)
        {
            isEnable = true;
            targetTemperature = endTemperature;
        }
    }
    public void UpdateLightFire(float t)
    {
        SetTemperature(Mathf.Lerp(0, targetTemperature * 0.2f, t));
        SetFireIntensity(Mathf.Lerp(0, maxIntensity0, t));
    }
    public void FireDecay()
    {
        isEnable = false;

        SetTemperature(0);
        SetFireIntensity(0);
    }
    #endregion
    #region Горение
    private GeneralTime.TimeUnityEvent unityEvent;

    public void StartFire(Times fireDuration)
    {
        if(!isEnable)
            isEnable = true;

        this.fireStart = GeneralTime.Instance.globalTime;
        this.fireDuration = fireDuration;
        this.fireEnd = fireStart + fireDuration;

        unityEvent = new GeneralTime.TimeUnityEvent();
        unityEvent.AddEvent(GeneralTime.TimeUnityEvent.EventType.ExecuteInTime, fireEnd, null, UpdateFire, StopFire);
        GeneralTime.Instance.AddEvent(unityEvent);

        EnableParticles();

        zone.TemperatureInZone = currentTemperature;
        zone.IsEnable = true;
    }
    public void UpdateFire(Times time)
    {
        currentTime = fireEnd - time;

        UpdateItems();

        float value = Mathf.Lerp(maxIntensity0, maxIntensity1, Mathf.PerlinNoise(randomValue, Time.time));

        if(currentTemperature != targetTemperature)
            SetTemperature(currentTemperature + 0.01f);

        zone.TemperatureInZone = currentTemperature;
    }
    public void StopFire()
    {
        if (isEnable)
        {
            DisableParticles();
            isEnable = false;
            zone.IsEnable = false;

            GeneralAvailability.PlayerUI.BreakFireMenu();

            Debug.LogError("Fire End");
        }
    }
    #endregion
    #endregion

    #region Observe
    public override void StartObserve()
    {
        base.StartObserve();
        if (isEnable)
        {
            if(isEnableOnAwake)
                GeneralAvailability.TargetPoint.SetTooltipAddText(currentTime.ToStringSimplification(isInfinity : true)).ShowAddToolTip();
            else
                GeneralAvailability.TargetPoint.SetTooltipAddText(ToolTip).ShowAddToolTip();
        }
        View();
    }
    public override void Observe()
    {
        if (isEnable)
        {
            base.Observe();
            
            if(!isEnableOnAwake)
                GeneralAvailability.TargetPoint.SetTooltipAddText(ToolTip);
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
        {
            InteractionButton.pointer.AddPressListener(OpenCookingWindow);
        }
        else
        {
            InteractionButton.pointer.AddPressListener(OpenIgnitionWindow);
        }
    }


    protected void OpenIgnitionWindow()
    {
        requirementsValues = new RequirementsIgnition(GeneralAvailability.PlayerInventory, CheckRequirements);

        CheckRequirements();

        WindowIgnition.Setup(requirementsValues);

        WindowIgnition.onStart = StartIgnition;

        GeneralAvailability.PlayerUI.OpenIgnition();
    }
    private void OpenCookingWindow()
    {
        GeneralAvailability.PlayerUI.OpenFireMenu(this);
    }
    #endregion

    private bool isCanIgnition = false;

    private void StartIgnition()
    {
        if (isCanIgnition)
        {
            WindowIgnition.HideWindow();
            if (!IsHoldIgnitionProccess)
            {
                GeneralAvailability.TargetPoint.ShowHightBar();
                holdIgnitionCoroutine = StartCoroutine(Ignition());
            }
        }
        else
        {
            Debug.LogError("ERROR", gameObject);
        }
    }
    private IEnumerator Ignition()
    {

        GeneralTime.Instance.IsStopped = true;

        Times global = GeneralTime.Instance.globalTime;

        int aTime = global.TotalSeconds;
        global += kindleTime;
        int bTime = global.TotalSeconds;
        int secs = 0;

        float currentTime = Time.deltaTime;

        LightFire(targetTemperature);

        float breakTime = -1;

        float chance = Random.Range(0, 100);
        Debug.LogError(chance + " - " + successChance);
        if (chance <= successChance)
        {

        }
        else
        {
            breakTime = Random.Range(0.2f, 0.8f);
        }


        while (currentTime < holdTime)
        {
            float progress = currentTime / holdTime;

            secs = (int)Mathf.Lerp(aTime, bTime, progress);
            GeneralTime.Instance.ChangeTimeOn(secs);

            GeneralAvailability.TargetPoint.SetBarHightValue(progress, "%");

            UpdateLightFire(progress);

            currentTime += Time.deltaTime;

            if (breakTime != -1)
            {
                if (progress >= breakTime)
                {
                    BreakHold();
                }
            }

            yield return null;
        }

        StartFire(fireDuration);

        StopHold();

        requirementsValues.Exchange();
    }
    public void BreakHold()
    {
        StopHold();

        FireDecay();

        requirementsValues.PartlyExchange();
    }
    public void StopHold()
    {
        if (IsHoldIgnitionProccess)
        {
            StopCoroutine(holdIgnitionCoroutine);
            holdIgnitionCoroutine = null;

            GeneralTime.Instance.IsStopped = false;

            GeneralAvailability.TargetPoint.HideHightBar();

            WindowIgnition.Back();
        }
    }

    private void CheckRequirements()
    {
        FireStarterSD starter = null;
        FireTinderSD tinder = null;
        FireFuelSD fuel = null;
        FireAccelerantSD accelerant = null;

        #region Checks
        Item starterItem = requirementsValues.starters.CurrentItem;
        if (starterItem != null)
        {
            starter = starterItem.itemData.scriptableData as FireStarterSD;
        }
        Item tinderItem = requirementsValues.tinders.CurrentItem;
        if (tinderItem != null)
        {
            tinder = tinderItem.itemData.scriptableData as FireTinderSD;
        }
        Item fuelItem = requirementsValues.fuels.CurrentItem;
        if (fuelItem != null)
        {
            fuel = fuelItem.itemData.scriptableData as FireFuelSD;
        }
        Item accelerantItem = requirementsValues.accelerants.CurrentItem;
        if (accelerantItem != null)
        {
            accelerant = accelerantItem.itemData.scriptableData as FireAccelerantSD;
        }
        #endregion

        float baseChance = GeneralAvailability.Player.Status.baseChanceIgnition;

        if (starter == null)
        {
            successChance = 0;
        }
        else
        {
            successChance = baseChance + starter.chance;
            if (tinder != null) successChance += tinder.chance;
            if (fuel != null) { successChance += fuel.chance; fireDuration = fuel.addFireTime; targetTemperature = fuel.addTemperature; }
            if (accelerant != null) successChance += accelerant.chance;
            successChance = Mathf.Clamp(successChance, 0, 100);

            kindleTime = starter.kindleTime;

            if (accelerant != null)
            {
                holdTime = accelerant.holdTime;
            }
            else
            {
                holdTime = starter.holdTime;
            }
        }

        WindowIgnition.UpdateUI(baseChance, successChance, starter == null ? SymbolCollector.DASH.ToString() : starter.kindleTime.ToStringSimplification(), fireDuration.ToStringSimplification());

        isCanIgnition = starter != null && tinder != null && fuel != null;
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
//Проверить на розжиг на нескольких айтемов