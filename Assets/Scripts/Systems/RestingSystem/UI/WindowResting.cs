using Sirenix.OdinInspector;

using System.Collections;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

//пробразовать к Times
public class WindowResting : WindowUI
{
    public UnityAction onBack;

    [SerializeField] private Button buttonBack;
    [SerializeField] private Pointer panelBack;

    [SerializeField] private Toggle toggleSleep;
    [SerializeField] private Toggle togglePass;
    [Space]
    [SerializeField] private GameObject breakPanel;
    [SerializeField] private Button breakButton;

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

    private Coroutine skipCoroutine = null;
    public bool IsSkipProccess => skipCoroutine != null;

    private PanelRest rest;


    private void Awake()
    {
        buttonBack.onClick.AddListener(Cancel);
        panelBack.onPressed.AddListener(Cancel);

        toggleSleep.onValueChanged.AddListener(TogglesChanged);
        togglePass.onValueChanged.AddListener(TogglesChanged);

        sleep.onRest += StartSkip;
        pass.onRest += StartSkip;
    }

    public override void ShowWindow()
    {
        base.ShowWindow();


        toggleSleep.isOn = true;
        TogglesChanged(toggleSleep.isOn);
        Setup();
    }

    public void Setup()
    {
        sleep.Setup(Status.maxSleepTime.TotalSeconds, Status.maxWaitSleeping, 3600);
        pass.Setup(Status.maxPassTime.TotalSeconds, Status.maxWaitPass, 300);

        breakButton.onClick.RemoveAllListeners();
        breakButton.onClick.AddListener(Break);
    }

    private void TogglesChanged(bool trigger)
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

    private void StartSkip(PanelRest rest, Times skipTime)
    {
        if (!IsSkipProccess)
        {
            float maxWait;

            this.rest = rest;

            if (toggleSleep.isOn)
            {
                Status.states.CurrentState = PlayerState.Sleeping;

                maxWait = Status.maxWaitSleeping;
            }
            else
            {
                Status.states.CurrentState = PlayerState.Resting;

                maxWait = Status.maxWaitPass;
            }

            rest.Enable(false);
            

            skipCoroutine = StartCoroutine(SkipTime(skipTime, maxWait));
        }
    }
    private IEnumerator SkipTime(Times skipTime, float maxWait)
    {
        breakPanel.SetActive(true);

        GeneralTime.Instance.IsStopped = true;

        Times global = GeneralTime.Instance.globalTime;

        int aTime = global.TotalSeconds;
        global += skipTime;
        int bTime = global.TotalSeconds;
        int secs = 0;

        float currentTime = Time.deltaTime;

        float lastSliderValue = rest.slider.value;

        float waitTime = Mathf.Lerp(1, maxWait, lastSliderValue / rest.slider.maxValue);

        while (currentTime < waitTime)
        {
            float progress = currentTime / waitTime;

            secs = (int)Mathf.Lerp(aTime, bTime, progress);
            rest.slider.value = Mathf.Lerp(lastSliderValue, 0, progress);
            GeneralTime.Instance.ChangeTimeOn(secs);

            currentTime += Time.deltaTime;

            yield return null;
        }

        rest.Enable(true);

        Cancel();

        StopSkip();
    }
    private void StopSkip()
    {
        if (IsSkipProccess)
        {
            StopCoroutine(skipCoroutine);
            skipCoroutine = null;

            GeneralTime.Instance.IsStopped = false;

            status.states.CurrentState = PlayerState.Standing;
        }
    }

    private void Break()
    {
        rest.Enable(true);
        StopSkip();
        breakPanel.SetActive(false);
    }

    private void Cancel()
    {
        rest = null;
        breakPanel.SetActive(false);
        onBack?.Invoke();
    }
}