using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WindowHarvesting : WindowUI
{
    public UnityAction onBack;

    [SerializeField] private Pointer background;
    [SerializeField] private Pointer buttonBack;
    [SerializeField] private Pointer buttonHarvest;
    [Space]
    [SerializeField] private TMPro.TextMeshProUGUI tittleText;
    [SerializeField] private TMPro.TextMeshProUGUI timeText;
    [SerializeField] private HarvestingResultUI result;
    [SerializeField] private HarvestingToolUI tool;

    private bool isFirstTime = true;

    private Coroutine holdIgnitionCoroutine = null;
    public bool IsHoldIgnitionProccess => holdIgnitionCoroutine != null;

    private Times kindleTime;
    private float holdTime = 1f;

    private Inventory inventory;
    private HarvestingObject harvesting;
    private HarvestingSD data;
    private int currentTool = -1;

    private List<Item> tools = new List<Item>();

    public void SetHarvesting(HarvestingObject harvesting)
    {
        this.harvesting = harvesting;

        if (isFirstTime)
        {
            Setup();
            isFirstTime = false;
        }

        OpenWindow();
    }

    public void OpenWindow()
    {
        UpdateUI();

        ShowWindow();
    }
    private void Setup()
    {
        buttonBack.onPressed.AddListener(Back);
        background.AddPressListener(Back);

        buttonHarvest.AddPressListener(StartHold);

        tool.onLeft += ToolLeft;
        tool.onRight += ToolRight;

        inventory = GeneralAvailability.PlayerInventory;
    }
    private void UpdateUI()
    {
        data = harvesting.Data;
        tittleText.text = data.objectName;

        tools.Clear();
        for (int i = 0; i < data.addDatas.Count; i++)
        {
            ToolItemSD toolItem = data.addDatas[i].tool;
            if (toolItem != null)
            {
                tools.AddRange(GeneralAvailability.PlayerInventory.GetAllBySD(toolItem));
            }
            else
            {
                tools.Add(null);
            }
        }

        if (tools.Count > 1)
        {
            tool.EnableLeft(false);
            tool.EnableRight(true);
        }
        else
        {
            tool.EnableLeft(false);
            tool.EnableRight(false);
        }

        currentTool = 0;
        UpdateToolsUI();

        result.SetItem(data.yields[data.yields.Count - 1]);//make loop
    }
    private void UpdateToolsUI()
    {
        ToolItemSD toolSD;
        if (tools[currentTool] != null)
            toolSD = tools[currentTool].itemData.scriptableData as ToolItemSD;
        else
            toolSD = null;

        tool.SetTool(toolSD);
        HarvestingSD.HarvestingData harvesting = data.addDatas.Find((x) => x.tool == toolSD);
        timeText.text = harvesting.timeLimits.GetTimes();
        holdTime = harvesting.holdTime;
        kindleTime.TotalSeconds = harvesting.timeLimits.GetTotalTime();
    }

    private void StartHold()
    {
        GeneralTime.Instance.SkipSetup(start: StartSkip, end: EndSkip, progress: UpdateSkip).StartSkip(kindleTime, holdTime);
    }
    private void StartSkip()
    {
        HideWindow();
        GeneralAvailability.TargetPoint.SetBarHightValue(0, "%").ShowHightBar();
    }
    private void UpdateSkip(float progress)
    {
        GeneralAvailability.TargetPoint.SetBarHightValue(progress, "%");
    }
    private void EndSkip()
    {
        Exchange();
        GeneralAvailability.TargetPoint.HideHightBar();
    }


    private void Exchange()
    {
        for (int i = 0; i < data.yields.Count; i++)
        {
            ItemDataWrapper itemData = data.yields[i].item.Copy();

            if (data.yields[i].isRandom)
            {
                itemData.CurrentStackSize = Random.Range(itemData.CurrentStackSize, data.yields[i].maxStackSize);
            }

            inventory.AddItem(itemData);
        }

        if (harvesting.IsBreakable)
            ObjectPool.ReturnGameObject(harvesting.gameObject);

        Back();
    }

    private void ToolLeft()
    {
        currentTool--;

        tool.EnableRight(true);

        if (currentTool == 0)//hands
        {
            tool.EnableLeft(false);
        }

        UpdateToolsUI();
    }
    private void ToolRight()
    {
        currentTool++;

        tool.EnableLeft(true);

        if (currentTool + 1 >= tools.Count)
        {
            tool.EnableRight(false);
        }

        UpdateToolsUI();
    }

    private void Back()
    {
        onBack?.Invoke();
    }
}