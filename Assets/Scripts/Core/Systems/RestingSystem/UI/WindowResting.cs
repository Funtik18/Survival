using Sirenix.OdinInspector;

using System.Collections;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

//пробразовать к Times
public class WindowResting : WindowUI
{
    public UnityAction onBack;
    public UnityAction onTakeIt;

    [SerializeField] private Button buttonBack;
    [SerializeField] private Pointer panelBack;
    [Space]
    [SerializeField] private Pointer takeIt;
    [Space]
    [SerializeField] private Toggle toggleSleep;
    [SerializeField] private Toggle togglePass;
    [Space]
    [SerializeField] private PanelRest sleep;
    [SerializeField] private PanelRest pass;


    private PlayerStatus status;
    private PlayerStatus Status 
    {
        get
        {
            if(status == null)
            {
                status = GeneralAvailability.Player.Status;
            }
            return status;
        }
    }


    private PanelRest rest;

    private void Awake()
    {
        buttonBack.onClick.AddListener(Cancel);
        panelBack.onPressed.AddListener(Cancel);

        takeIt.onPressed.AddListener(TakeIt);

        toggleSleep.onValueChanged.AddListener(TogglesChanged);
        togglePass.onValueChanged.AddListener(TogglesChanged);

        sleep.onRest += StartSkip;
        pass.onRest += StartSkip;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="useTakeButton"></param>
    /// <param name="type">Если == 0 значит показывает и sleep и rest панели, если == 1 то только sleep, если == 2 то только rest.</param>
    /// <returns></returns>
    public WindowResting Setup(bool useTakeButton = false, int type = 0)
    {
        takeIt.gameObject.SetActive(false);

        if (type == 0)
        {
            toggleSleep.isOn = true;

            sleep.Setup(Laws.Instance.maxSleepTime.TotalSeconds, Laws.Instance.waitRealTimeSleeping, 3600);
            pass.Setup(Laws.Instance.maxPassTime.TotalSeconds, Laws.Instance.waitRealTimePassing, 300);
        }
        else if(type == 1)
        {
            toggleSleep.isOn = true;

            toggleSleep.gameObject.SetActive(false);
            togglePass.gameObject.SetActive(false);


            sleep.Setup(Laws.Instance.maxSleepTime.TotalSeconds, Laws.Instance.waitRealTimeSleeping, 3600);

            if (useTakeButton != false)
                takeIt.gameObject.SetActive(useTakeButton);
        }
        else if(type == 2)
        {
            togglePass.isOn = true;

            toggleSleep.gameObject.SetActive(false);
            togglePass.gameObject.SetActive(false);

            pass.Setup(Laws.Instance.maxPassTime.TotalSeconds, Laws.Instance.waitRealTimePassing, 300);
        }

        return this;
    }
    public override void HideWindow()
    {
        base.HideWindow();
        toggleSleep.gameObject.SetActive(true);
        togglePass.gameObject.SetActive(true);
    }


    private float lastSliderValue;
    private bool isSleep = true;
    private Times howLong;
    private void StartSkip(PanelRest rest, Times skipTime)
    {
        this.rest = rest;


        isSleep = toggleSleep.isOn;
        howLong = skipTime;

        if (isSleep)
        {
            Status.states.CurrentState = PlayerState.Sleeping;

            AdMobManager.Instance.ShowInterstitial();

            GeneralTime.Instance.SkipSetup(start: StartSkip, end: EndSkip, time: UpdateSKip).StartSkip(skipTime, Laws.Instance.waitRealTimeSleeping);
        }
        else
        {
            Status.states.CurrentState = PlayerState.Resting;

            lastSliderValue = rest.slider.value;

            GeneralAvailability.PlayerUI.ShowBreakButton().BreakPointer.AddPressListener(BreakSkip);
            GeneralTime.Instance.SkipSetup(start: StartSkip, end: EndSkip, brek: EndSkip, progress: UpdateSKip).StartSkip(skipTime, Laws.Instance.waitRealTimePassing);
        }
    }

    private void StartSkip()
    {
        if (isSleep)
        {
            GeneralAvailability.PlayerUI.sleepPanel.Enable(true);
        }
        else
        {
            GeneralAvailability.PlayerUI.blockPanel.Enable(true);
        }
    }
    private void UpdateSKip(float progress)
    {
        rest.slider.value = Mathf.Lerp(lastSliderValue, 0, progress);
    }
    private void UpdateSKip(Times time)
    {
        GeneralAvailability.PlayerUI.sleepPanel.UpdateUI(time.ToStringSimplification());
    }

    private void BreakSkip()
    {
        GeneralTime.Instance.BreakSkipTime();
    }

    private void EndSkip()
    {
        if (isSleep)
        {
            Cancel();
            GeneralAvailability.PlayerUI.sleepPanel.Enable(false);
        
            if (howLong.TotalSeconds >= 3600)
            {
                GlobalSaveLoader.Instance.StartSaveGame();
            }
        }
        else
        {
            GeneralAvailability.PlayerUI.HideBreakButton();
            GeneralAvailability.PlayerUI.blockPanel.Enable(false);
        }

        status.states.CurrentState = PlayerState.Standing;
    }

    private void TogglesChanged(bool trigger = false)
    {
        if (toggleSleep.isOn)
        {
            sleep.UpdateSlider();

            sleep.ShowWindow();
            pass.HideWindow();
        }
        else
        {
            pass.UpdateSlider();

            sleep.HideWindow();
            pass.ShowWindow();
        }
    }

    private void TakeIt()
    {
        takeIt.gameObject.SetActive(false);

        takeIt.onPressed.RemoveAllListeners();

        onTakeIt?.Invoke();
        Cancel();
    }

    private void Cancel()
    {
        rest = null;
        onBack?.Invoke();
    }
}