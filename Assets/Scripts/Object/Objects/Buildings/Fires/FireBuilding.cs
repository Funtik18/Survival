using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;
using UnityEngine.Events;

public class FireBuilding : BuildingObject
{
    public UnityAction<string> onFireDuration;
    public UnityAction<string> onFireTemperature;
    public UnityAction<float> onFireTemperaturePercent;

    public UnityAction onFireEnd;

    [PropertyOrder(-1)]
    [SerializeField] protected bool isEnableOnAwake = false;
    
    [PropertyOrder(2)]
    [SerializeField] protected List<ParticleSystem> particles = new List<ParticleSystem>();
    [PropertyOrder(2)]
    [SerializeField] private List<CookingSlot> slots = new List<CookingSlot>();
    [Space]
    [SerializeField] private float maxFireTemperature = 80;
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

    public bool IsFull
    {
        get
        {
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].IsEmpty)
                {
                    return false;
                }
            }
            return true;
        }
    }

    private Inventory Inventory => GeneralAvailability.PlayerInventory;

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

    private string ToolTip => CurrentTime.ToStringSimplification(true) + CurrentStringTemperature;

    private float successChance;

    private float holdTime = 1f;

    private float currentTemperature;
    public float CurrentTemperature
    {
        get => currentTemperature;
        set
        {
            currentTemperature = value;

            onFireTemperaturePercent?.Invoke(currentTemperature / maxFireTemperature);

            onFireTemperature?.Invoke(CurrentStringTemperature);
        }
    }
    public string CurrentStringTemperature => System.Math.Round(CurrentTemperature, 1) + SymbolCollector.CELSIUS;

    private float targetTemperature;





    private Times kindleTime;

    private Times fireStart;
    private Times fireDuration;
    private Times fireEnd;

    private Times currentTime;
    public Times CurrentTime
    {
        get => currentTime;
        set
        {
            currentTime = value;

            onFireDuration?.Invoke(currentTime.ToStringSimplification());
        }
    }

    protected bool isEnable = false;
    public bool IsEnable => isEnable;
    protected float randomValue;

    public CookingSlot GetEmptySlot() => slots.Find((x) => x.IsEmpty);


    private Coroutine holdIgnitionCoroutine = null;
    public bool IsHoldIgnitionProccess => holdIgnitionCoroutine != null;


    protected override void Awake()
    {
        base.Awake();

        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].SetOwner(this);
        }

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


    public void AddFuel(FireFuelSD fuel)
    {
        fireEnd += fuel.addFireTime;
        targetTemperature += fuel.addTemperature;
        unityEvent.SetTime(fireEnd);
    }
    public ItObject AddItemObjectOnSlot<ItObject>(Item item, CookingSlot slot) where ItObject : ItemObject
    {
        ItObject itemObject = ObjectPool.GetObject(item.itemData.scriptableData.model.gameObject).GetComponent<ItObject>();
        slot.SetItem(itemObject);

        return itemObject;
    }

    private void SetTemperature(float temperature)
    {
        CurrentTemperature = Mathf.Clamp(temperature, 0, targetTemperature);
    }
    private void SetFireIntensity(float value)
    {
        bonfireLight.intensity = value;
        zone.SetSize(value);
    }

    #region Observe
    public override void StartObserve()
    {
        base.StartObserve();
        if (isEnable)
        {
            if(isEnableOnAwake)
                GeneralAvailability.TargetPoint.SetTooltipAddText(CurrentTime.ToStringSimplification(isInfinity : true)).ShowAddToolTip();
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

    #region Ignition

    private bool isCanIgnition = false;

    #region Горение
    private GeneralTime.TimeUnityEvent unityEvent;

    public void StartFire(Times fireDuration)
    {
        if (!isEnable)
            isEnable = true;

        this.fireStart = GeneralTime.Instance.globalTime;
        this.fireDuration = fireDuration;
        this.fireEnd = fireStart + fireDuration;

        unityEvent = new GeneralTime.TimeUnityEvent();
        unityEvent.AddEvent(GeneralTime.TimeUnityEvent.EventType.ExecuteInTime, fireEnd, null, UpdateFire, StopFire);
        GeneralTime.Instance.AddEvent(unityEvent);

        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].StartWork();
        }

        EnableParticles();

        zone.TemperatureInZone = CurrentTemperature;
        zone.IsEnable = true;
    }
    public void UpdateFire(Times time)
    {
        CurrentTime = fireEnd - time;

        if (isEnable)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                slots[i].Work();
            }
        }

        float value = Mathf.Lerp(maxIntensity0, maxIntensity1, Mathf.PerlinNoise(randomValue, Time.time));

        if (CurrentTemperature != targetTemperature)
            SetTemperature(CurrentTemperature + 0.01f);

        zone.TemperatureInZone = CurrentTemperature;
    }
    public void StopFire()
    {
        if (isEnable)
        {
            DisableParticles();
            isEnable = false;
            zone.IsEnable = false;

            GeneralAvailability.PlayerUI.BreakFireMenu();

            for (int i = 0; i < slots.Count; i++)
            {
                slots[i].EndWork();
            }
        }
    }
    #endregion

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
        GeneralTime.Instance.IsTimeStopped = true;

        Times global = GeneralTime.Instance.globalTime;

        int aTime = global.TotalSeconds;
        global += kindleTime;
        int bTime = global.TotalSeconds;
        int secs = 0;

        float currentTime = Time.deltaTime;

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

            SetTemperature(Mathf.Lerp(CurrentTemperature, targetTemperature, progress));
            SetFireIntensity(Mathf.Lerp(0, maxIntensity0, progress));

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

        SetTemperature(0);
        SetFireIntensity(0);

        requirementsValues.PartlyExchange();
    }
    public void StopHold()
    {
        if (IsHoldIgnitionProccess)
        {
            StopCoroutine(holdIgnitionCoroutine);
            holdIgnitionCoroutine = null;

            GeneralTime.Instance.IsTimeStopped = false;

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

    public void ClearUIActions()
    {
        onFireDuration = null;
        onFireTemperature = null;
        onFireTemperaturePercent = null;
    }
}