using System.Collections;

using UnityEngine;
using UnityEngine.Events;

public class ItemObjectLiquidContainer : ItemObject
{
    private UnityAction onStageChanged;

    [SerializeField] private GameObject emptyObject;
    [SerializeField] private GameObject withSnowObject;
    [SerializeField] private GameObject withWaterObject;

    [HideInInspector] public ItemDataWrapper liquid;

    private CookingSlot slot;

    public bool IsProccessing
    {
        get
        {
            if (slot == null)
                return false;

            return slot.IsWorking;
        }
    }

    private int currentState = 0;

    private LiquidStage currentStage = LiquidStage.None;
    public LiquidStage CurrentStage
    {
        get => currentStage;
        set
        {
            currentStage = value;
            onStageChanged?.Invoke();
        }
    }

    private LiquidState state = LiquidState.WithoutLiqued;

    private float duration;
    
    private Times currentTime;
    private Times showTime;

    private float progress;

    private string tooltip;
    private string phrase;

    private float volume = -1;
    public float Volume
    {
        get
        {
            if(volume == -1)
            {
                volume = (itemData.scriptableData as ToolContainerItemSD).volume;
            }
            return volume;
        }
    }

    public ItemDataWrapper GetItem()
    {
        if(state == LiquidState.WithoutLiqued)
        {
            return null;
        }
        else if(state == LiquidState.WithLiquedUnSafe)
        {
            return ItemsData.Instance.GetWater(liquid.CurrentWeight, false);
        }
        else if(state == LiquidState.WithLiquedPotable)
        {
            return ItemsData.Instance.GetWater(liquid.CurrentWeight, true);
        }
        else if(state == LiquidState.WithSnow)
        {
            return null;
        }

        return null;
    }

    public void SetSlot(CookingSlot slot)
    {
        this.slot = slot;
    }

    public void StartProccess()
    {
        if (liquid.scriptableData is WaterItemSD)
        {
            StartBoiling();
        }
        else
        {
            StartMelting();
        }
    }

    private void StartMelting()
    {
        duration = Volume * Laws.Instance.meltTime.TotalSeconds;

        currentTime = new Times();
        showTime = new Times();

        CurrentStage = LiquidStage.Melting;
        state = LiquidState.WithSnow;

        IsWithSnow();
    }

    private void StartBoiling()
    {
        duration = Volume * Laws.Instance.boilsTime.TotalSeconds;

        currentTime = new Times();
        showTime = new Times();

        CurrentStage = LiquidStage.Boiling;
        state = LiquidState.WithLiquedUnSafe;

        IsWithWater();
    }

    public override void UpdateItem()
    {
        if (CurrentStage != LiquidStage.None)
        {
            currentTime.TotalSeconds += 1;
            progress = currentTime.TotalSeconds / duration;

            if (CurrentStage == LiquidStage.Melting)
            {
                phrase = " left before Boiled";

                if (progress >= 1f)
                {
                    duration = Volume * Laws.Instance.boilsTime.TotalSeconds;

                    currentTime.TotalSeconds = 0;

                    CurrentStage = LiquidStage.Boiling;
                    state = LiquidState.WithLiquedUnSafe;

                    IsWithWater();
                }
            }
            else if (CurrentStage == LiquidStage.Boiling)
            {
                phrase = " left before Ready";

                if (progress >= 1f)
                {
                    duration = Laws.Instance.evaporationTime.TotalSeconds;

                    currentTime.TotalSeconds = 0;

                    CurrentStage = LiquidStage.Evaporating;
                    state = LiquidState.WithLiquedPotable;
                }
            }
            else if (CurrentStage == LiquidStage.Evaporating)
            {
                phrase = " left before Evaporated";

                if (progress >= 1f)
                {
                    CurrentStage = LiquidStage.None;
                    state = LiquidState.WithoutLiqued;

                    IsEmpty();
                }
            }

            showTime.TotalSeconds = (int)duration - currentTime.TotalSeconds;

            tooltip = showTime.ToStringSimplification(true) + "  " + liquid.CurrentWeight + phrase;
        }
    }

    public void ActionItem()
    {
        slot.onSlotEndWork = BreakSkipTime;

        GeneralTime.Instance.SkipSetup(start: StartSkip, end: EndSkip).StartSkip(showTime, Laws.Instance.waitRealTimePassTime);
    }
    private void StartSkip()
    {
        GeneralAvailability.PlayerUI.blockPanel.Enable(true);
    }
    private void EndSkip()
    {
        GeneralAvailability.PlayerUI.blockPanel.Enable(false);
    }
    private void BreakSkipTime()
    {
        if (GeneralTime.Instance.IsSkipProccess())
        {
            GeneralTime.Instance.BreakSkipTime();
        }
    }


    public override void StartObserve()
    {
        base.StartObserve();
        if (CurrentStage != LiquidStage.None)
        {
            GeneralAvailability.TargetPoint.ShowAddToolTip();
        }
    }
    public override void Observe()
    {
        base.Observe();
        if (CurrentStage != LiquidStage.None)
        {
            GeneralAvailability.TargetPoint.SetTooltipAddText(tooltip);
        }
    }
    public override void EndObserve()
    {
        base.EndObserve();
        if (CurrentStage != LiquidStage.None)
        {
            GeneralAvailability.TargetPoint.HideAddToolTip();
        }
    }


    public void SetLiquid(ItemSD liquedSD, float litres)
    {
        liquid.scriptableData = liquedSD;
        liquid.CurrentWeight = litres;
    }

    public void IsEmpty()
    {
        if (currentState != 0)
        {
            currentState = 0;
            emptyObject.SetActive(true);
            withSnowObject.SetActive(false);
            withWaterObject.SetActive(false);
        }
    }
    public void IsWithSnow()
    {
        if (currentState != 1)
        {
            currentState = 1;
            emptyObject.SetActive(false);
            withSnowObject.SetActive(true);
            withWaterObject.SetActive(false);
        }
    }
    public void IsWithWater()
    {
        if (currentState != 2)
        {
            currentState = 2;
            emptyObject.SetActive(false);
            withSnowObject.SetActive(false);
            withWaterObject.SetActive(true);
        }
    }


    public enum LiquidStage
    {
        None,
        Melting,
        Boiling,
        Evaporating,
    }
    public enum LiquidState
    {
        WithSnow,
        WithLiquedUnSafe,
        WithLiquedPotable,
        WithoutLiqued,
    }
}