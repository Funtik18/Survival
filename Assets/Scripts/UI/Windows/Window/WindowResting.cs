using System.Collections;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

//пробразовать к Times
public class WindowResting : WindowUI
{
    public UnityAction onRest;
    public UnityAction onBack;

    [SerializeField] private Button buttonRest;
    [SerializeField] private Button buttonBack;

    [SerializeField] private TMPro.TextMeshProUGUI textTitte;
    [SerializeField] private TMPro.TextMeshProUGUI textTime;

    [SerializeField] private Button left;
    [SerializeField] private Slider slider;
    [SerializeField] private Button right;
    [Space]
    [SerializeField] private GameObject breakPanel;
    [SerializeField] private Button breakButton;


    private Times skipTime;

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


    private void Awake()
    {
        buttonBack.onClick.AddListener(Cancel);
        buttonRest.onClick.AddListener(Rest);

        left.onClick.AddListener(Left);
        slider.onValueChanged.AddListener(UpdateSlider);
        right.onClick.AddListener(Right);
    }

    public override void ShowWindow()
    {
        base.ShowWindow();
        Setup();
    }

    public void Setup()
    {
        Times times = GeneralTime.Instance.globalTime;

        int min = 0;
        int max = min + Status.maxResting.TotalSeconds;

        slider.minValue = min;
        slider.maxValue = max;

        slider.value = max / 2;

        breakButton.onClick.AddListener(StopSkip);

        UpdateSlider(slider.value);
    }

    private void StartSkip()
    {
        if (!IsSkipProccess)
        {
            skipCoroutine = StartCoroutine(SkipTime());
        }
    }
    private IEnumerator SkipTime()
    {
        Status.states.CurrentState = PlayerState.Sleeping;

        Enable(false);

        GeneralTime.Instance.IsStopped = true;

        Times global = GeneralTime.Instance.globalTime;

        int aTime = global.TotalSeconds;
        global += skipTime;
        int bTime = global.TotalSeconds;
        int secs = 0;

        float lastSliderValue = slider.value;
        float currentTime = Time.deltaTime;

        float waitTime = Mathf.Lerp(1, Status.maxWaitResting, lastSliderValue / slider.maxValue);

        while (currentTime < waitTime)
        {
            float progress = currentTime / waitTime;

            secs = (int)Mathf.Lerp(aTime, bTime, progress);
            slider.value = Mathf.Lerp(lastSliderValue, 0, progress);
            GeneralTime.Instance.ChangeTimeOn(secs);

            currentTime += Time.deltaTime;

            yield return null;
        }

        StopSkip();
    }
    private void StopSkip()
    {
        if (IsSkipProccess)
        {
            StopCoroutine(skipCoroutine);
            skipCoroutine = null;

            GeneralTime.Instance.IsStopped = false;

            Status.states.CurrentState = PlayerState.Standing;

            Cancel();

            Enable(true);
        }
    }


    private void UpdateSlider(float value)
    {
        Times times = new Times();
        times.TotalSeconds = (int)value;
        textTime.text = times.ToStringSimplification();
    }

    private void Enable(bool trigger)
    {
        buttonRest.interactable = trigger;
        buttonBack.interactable = trigger;
        left.interactable = trigger;
        slider.interactable = trigger;
        right.interactable = trigger;

        breakPanel.SetActive(!trigger);
    }


    private void Left()
    {
        slider.value -= 3600f;
    }
    private void Right()
    {
        slider.value += 3600f;
    }

    private void Cancel()
    {
        onBack?.Invoke();
    }
    private void Rest()
    {
        skipTime.TotalSeconds = (int) slider.value;
        StartSkip();

        onRest?.Invoke();
    }
}