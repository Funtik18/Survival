using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WindowHarvesting : WindowUI
{
    public UnityAction onBack;
    public UnityAction onHarvestingCompletely;

    [SerializeField] private ProgressBar barRadial;
    [Space]
    [SerializeField] private Pointer background;
    [SerializeField] private CustomPointer buttonBack;
    [Space]
    [SerializeField] private TMPro.TextMeshProUGUI tittleText;
    [SerializeField] private TMPro.TextMeshProUGUI timeText;
    [SerializeField] private HarvestingResultUI result;


    private Coroutine holdIgnitionCoroutine = null;
    public bool IsHoldIgnitionProccess => holdIgnitionCoroutine != null;

    private Times kindleTime;
    private float holdTime = 1f;

    private Inventory inventory;
    private HarvestingObject harvesting;

    private void Awake()
    {
        buttonBack.pointer.onPressed.AddListener(Back);

        background.AddDoublePressListener(StartHold);
    }

    public void Setup(Inventory inventory)
    {
        this.inventory = inventory;
    }

    public void SetHarvesting(HarvestingObject harvesting)
    {
        this.harvesting = harvesting;

        OpenWindow();
    }

    public void OpenWindow()
    {
        UpdateUI();

        ShowWindow();
    }
    private void UpdateUI()
    {
        HarvestingSD data = harvesting.data;
        tittleText.text = data.objectName;
        timeText.text = data.howLong.ToStringSimplification();

        result.SetItem(data.items[data.items.Count - 1]);//make loop

        holdTime = data.holdTime;
    }


    public void StartHold()
    {
        HideWindow();
        if (!IsHoldIgnitionProccess)
        {
            barRadial.ShowBar();
            holdIgnitionCoroutine = StartCoroutine(Hold());
        }
    }
    private IEnumerator Hold()
    {
        GeneralTime.Instance.IsStopped = true;
        Times global = GeneralTime.Instance.globalTime;

        int aTime = global.TotalSeconds;
        int bTime = aTime + kindleTime.TotalSeconds;
        int secs = 0;

        float currentTime = Time.deltaTime;
        while (currentTime < holdTime)
        {
            float progress = currentTime / holdTime;

            secs = (int)Mathf.Lerp(aTime, bTime, progress);
            GeneralTime.Instance.ChangeTimeOn(secs);

            barRadial.UpdateFillAmount(progress, "%");

            currentTime += Time.deltaTime;

            yield return null;
        }

        Exchange();

        GeneralTime.Instance.IsStopped = false;

        onHarvestingCompletely?.Invoke();

        StopHold();
    }
    public void StopHold()
    {
        if (IsHoldIgnitionProccess)
        {
            StopCoroutine(holdIgnitionCoroutine);
            holdIgnitionCoroutine = null;

            barRadial.HideBar();
        }
    }

    private void Exchange()
    {
        for (int i = 0; i < harvesting.data.items.Count; i++)
        {
            ItemDataWrapper copyItem = harvesting.data.items[i].Copy();

            inventory.AddItem(copyItem);
        }

        if (harvesting.IsBreakable)
            Destroy(harvesting.gameObject);
    }


    private void Back()
    {
        onBack?.Invoke();
    }
}