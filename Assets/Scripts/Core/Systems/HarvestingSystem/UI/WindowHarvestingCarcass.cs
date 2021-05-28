using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WindowHarvestingCarcass : WindowUI
{
    public UnityAction onBack;

    [SerializeField] private Pointer background;
    [SerializeField] private Pointer buttonBack;
    [SerializeField] private Pointer buttonHarvest;
    [SerializeField] private GameObject harvest;
    [Space]
    [SerializeField] private TMPro.TextMeshProUGUI tittleText;
    [SerializeField] private TMPro.TextMeshProUGUI timeText;
    [SerializeField] private HarvestingToolUI tool;
    [Space]
    [SerializeField] private HarvestingCarcassResultUI meat;

    private bool isFirstTime = true;

    private Times meatTime;
    private Times kindleTime;
    private float meatHoldTime = 1f;
    private float kindleHoldTime = 1f;

    private Inventory inventory;
    private Animal harvesting;
    private AnimalSD data;
    private ToolItemSD toolItem;
    private int currentTool;

    private List<Item> tools = new List<Item>();
    private HarvestingSD.HarvestingData meatHarvesting;

    public void SetHarvesting(Animal harvesting)
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
        UIInit();

        ShowWindow();
    }

    private void Setup()
    {
        buttonBack.onPressed.AddListener(Back);
        background.AddPressListener(Back);

        buttonHarvest.AddPressListener(StartHold);

        tool.onLeft += ToolLeft;
        tool.onRight += ToolRight;

        meat.onLeft += UpdateUI;
        meat.onRight += UpdateUI;

        inventory = GeneralAvailability.PlayerInventory;
    }
    private void UIInit()
    {
        meat.Setup(harvesting.Meat, "KG");
        data = harvesting.SData;
        tittleText.text = data.animalName;

        tools.Clear();
        for (int i = 0; i < data.addDatasMeatKG.Count; i++)
        {
            ToolItemSD toolItem = data.addDatasMeatKG[i].tool;
            if (toolItem != null)
                tools.AddRange(GeneralAvailability.PlayerInventory.GetAllBySD(toolItem));
            else
                tools.Add(null);
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

        UpdateTool();
        UpdateUI();
    }
    private void UpdateUI()
    {
        meatHoldTime = meatHarvesting.holdTime;
        meatTime.TotalSeconds = (int)(meat.CurrentValue * meatHarvesting.timeLimits.GetTotalTime());

        kindleTime = meatTime;
        kindleHoldTime = meatHoldTime;

        timeText.text = kindleTime.ToStringSimplification();

        harvest.SetActive(kindleTime.TotalSeconds != 0);
    }
    private void UpdateTool()
    {
        if (tools[currentTool] != null)
            toolItem = tools[currentTool].itemData.scriptableData as ToolItemSD;
        else
            toolItem = null;

        tool.SetTool(toolItem);

        meatHarvesting = data.addDatasMeatKG.Find((x) => x.tool == toolItem);

        UpdateUI();
    }


    private void StartHold()
    {
        GeneralTime.Instance.SkipSetup(start: StartSkip, end: EndSkip, progress: UpdateSkip).StartSkip(kindleTime, kindleHoldTime);
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
        if(meat.CurrentValue > 0)
        {
            harvesting.Meat.CurrentBaseWeight -= meat.CurrentValue;

            ItemDataWrapper meatData = harvesting.Meat.Copy();
            meatData.CurrentBaseWeight = meat.CurrentValue;

            inventory.AddItem(meatData);
        }

        if (harvesting.IsBreakable && harvesting.Meat.CurrentBaseWeight <= 0.05f)
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
        
        UpdateTool();
    }
    private void ToolRight()
    {
        currentTool++;

        tool.EnableLeft(true);

        if (currentTool + 1 >= tools.Count)
        {
            tool.EnableRight(false);
        }

        UpdateTool();
    }

    private void Back()
    {
        onBack?.Invoke();
    }
}