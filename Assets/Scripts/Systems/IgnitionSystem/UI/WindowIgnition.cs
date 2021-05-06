using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WindowIgnition : WindowUI
{
    public UnityAction onBack;
    public UnityAction onIgnitionCompletely;

    [SerializeField] private Pointer background;
    [SerializeField] private CustomPointer buttonBack;
    [SerializeField] private CustomPointer buttonHelp;

    [SerializeField] private IgnitionRequirementsUI requirements;
    [Space]
    [SerializeField] private TMPro.TextMeshProUGUI baseChanceText;
    [SerializeField] private TMPro.TextMeshProUGUI estimatedFireText;
    [SerializeField] private TMPro.TextMeshProUGUI estimatedFireDurationText;
    [SerializeField] private TMPro.TextMeshProUGUI chanceSuccessText;

    private FireBuilding fireBuilding;

    private bool isIgnitionCompletly = false;

    private bool isCanIgnition = false;
    private float holdTime = 1f;
    private Times kindleTime;
    private Times fireDuration;
    private float fireTemperature = 0f;

    private IgnitionRequirements requirementsValues;

    private Coroutine holdIgnitionCoroutine = null;
    public bool IsHoldIgnitionProccess => holdIgnitionCoroutine != null;

    private void Awake()
    {
        buttonBack.pointer.onPressed.AddListener(Back);

        background.AddDoublePressListener(StartHold);
    }

    public void Setup(Inventory inventory)
    {
        requirementsValues = new IgnitionRequirements(inventory);
        requirementsValues.onChanged = UpdateUI;
    }

    public void SetBuilding(FireBuilding building)
    {
        fireBuilding = building;

        isIgnitionCompletly = false;

        OpenWindow();
    }

    public void OpenWindow()
    {
        requirementsValues.Setup();
        requirements.Setup(requirementsValues);

        UpdateUI();

        ShowWindow();
    }


    private void UpdateUI()
    {
        FireStarterSD starter = null;
        FireTinderSD tinder = null;
        FireFuelSD fuel = null;
        FireAccelerantSD accelerant= null;

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

        float successChance;
        if(starter == null)
            successChance = 0;
        else
        {
            successChance = baseChance + starter.chance;
            if (tinder != null) successChance += tinder.chance;
            if (fuel != null) { successChance += fuel.chance; fireDuration = fuel.addFireTime; fireTemperature = fuel.addTemperature; }
            if (accelerant != null) successChance += accelerant.chance;
            successChance = Mathf.Clamp(successChance, 0, 100);

            kindleTime = starter.kindleTime;

            if(accelerant != null)
            {
                holdTime = accelerant.holdTime;
            }
            else
            {
                holdTime = starter.holdTime;
            }
        }

        baseChanceText.text = baseChance + "%";
        estimatedFireText.text = starter == null ? SymbolCollector.DASH.ToString() : starter.kindleTime.ToStringSimplification();
        estimatedFireDurationText.text = fireDuration.ToStringSimplification();
        chanceSuccessText.text = successChance + "%";

        isCanIgnition = starter != null && tinder != null && fuel != null;
    }


    public void StartHold()
    {
        if (isCanIgnition)
        {
            HideWindow();
            if (!IsHoldIgnitionProccess)
            {
                GeneralAvailability.TargetPoint.ShowHightBar();
                holdIgnitionCoroutine = StartCoroutine(Hold());
            }
        }
        else
        {
            Debug.LogError("ERROR");
        }
    }
    private IEnumerator Hold()
    {
        requirementsValues.Exchange();

        GeneralTime.Instance.IsStopped = true;

        Times global = GeneralTime.Instance.globalTime;

        int aTime = global.TotalSeconds;
        global += kindleTime;
        int bTime = global.TotalSeconds;
        int secs = 0;

        float currentTime = Time.deltaTime;

        fireBuilding.LightFire(fireTemperature);

        while (currentTime < holdTime)
        {
            float progress = currentTime / holdTime;

            secs = (int)Mathf.Lerp(aTime, bTime, progress);
            GeneralTime.Instance.ChangeTimeOn(secs);

            GeneralAvailability.TargetPoint.SetBarHightValue(progress, "%");

            fireBuilding.UpdateLightFire(progress);

            currentTime += Time.deltaTime;

            yield return null;
        }

        isIgnitionCompletly = true;
        onIgnitionCompletely?.Invoke();

        fireBuilding.StartFire(fireDuration);

        GeneralTime.Instance.IsStopped = false;

        Back();
    }
    public void StopHold()
    {
        if (IsHoldIgnitionProccess)
        {
            StopCoroutine(holdIgnitionCoroutine);
            holdIgnitionCoroutine = null;

            GeneralAvailability.TargetPoint.HideHightBar();
        
            if(isIgnitionCompletly == false)
            {
                Debug.LogError("Chnace");
                fireBuilding.FireDecay();
            }
        }
    }

    private void Back()
    {
        StopHold();
        onBack?.Invoke();
    }

    public class IgnitionRequirements
    {
        public UnityAction onChanged;

        public RequirementsItem starters;
        public RequirementsItem tinders;
        public RequirementsItem fuels;
        public RequirementsItem accelerants;

        private Inventory inventory;

        public bool Contains
        {
            get
            {
                if (inventory.ContainsType<FireStarterSD>() && inventory.ContainsType<FireTinderSD>() && inventory.ContainsType<FireFuelSD>())
                    return true;
                return false;
            }
        }

        public IgnitionRequirements(Inventory inventory)
        {
            this.inventory = inventory;
        }
        public void Setup()
        {
            starters = new RequirementsItem(inventory.GetAllBySD<FireStarterSD>());
            tinders = new RequirementsItem(inventory.GetAllBySD<FireTinderSD>());
            fuels = new RequirementsItem(inventory.GetAllBySD<FireFuelSD>());
            accelerants = new RequirementsItem(inventory.GetAllBySD<FireAccelerantSD>());

            starters.onValueChanged += Change;
            tinders.onValueChanged += Change;
            fuels.onValueChanged += Change;
            accelerants.onValueChanged += Change;
        }

        public void Exchange()
        {
            inventory.RemoveItem(starters.CurrentItem, 1);
            inventory.RemoveItem(tinders.CurrentItem, 1);
            inventory.RemoveItem(fuels.CurrentItem, 1);
            inventory.RemoveItem(accelerants.CurrentItem, 1);
        }

        private void Change()
        {
            onChanged?.Invoke();
        }
    }
}
public class RequirementsItem
{
    public UnityAction onValueChanged;

    public List<Item> requirements;

    public Item CurrentItem
    {
        get
        {
            if (CurrentIndex != -1)
                return requirements[CurrentIndex];
            return null;
        }
    }

    private int currentIndex;
    public int CurrentIndex
    {
        get
        {
            if (requirements.Count > 0) return currentIndex;
            return -1;
        }
        set
        {
            currentIndex = value;

            onValueChanged?.Invoke();
        }
    }

    public RequirementsItem(List<Item> requirements)
    {
        this.requirements = requirements;
    }
}