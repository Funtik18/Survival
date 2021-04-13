using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IgnitionRequirementUI : WindowUI
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private Image rejectIcon;
    [Space]
    [SerializeField] private GameObject btnLeft;
    [SerializeField] private GameObject btnRight;
    [Space]
    [SerializeField] private TMPro.TextMeshProUGUI itemText;
    [SerializeField] private TMPro.TextMeshProUGUI itemHelperText;
    [Space]
    [SerializeField] private TMPro.TextMeshProUGUI indicatorText;
    [SerializeField] private Transform indicatorsParent;
    [SerializeField] private IndicatorUI indicatorPrefab;
    [Space]
    [SerializeField] private bool isLoop = false;
    [SerializeField] private bool isInverse = false;
    [SerializeField] private Animator animator;

    private RequirementsItem requirements;
    private int CurrentIndex { get => requirements.CurrentIndex; set => requirements.CurrentIndex = value; }
    private List<Item> items => requirements.requirements;

    private List<IndicatorUI> indicators = new List<IndicatorUI>();

    public void Setup(RequirementsItem requirements, int defaultIndex = 0)
    {
        this.requirements = requirements;

        requirements.CurrentIndex = defaultIndex;

        ReCreateIndicators();
        UpdateIndicators();

        UpdateUI();
    }

    #region Left Right
    public void Left()
    {
        if (isLoop)
            LeftLoop();
        else
            LeftClamp();
    }
    public void Right()
    {

        if (isLoop)
            RightLoop();
        else
            RightClamp();
    }

    private void LeftLoop()
    {
        if (CurrentIndex == 0)
            CurrentIndex = items.Count - 1;
        else
            CurrentIndex--;

        UpdateUI();
        AnimatorWork(false);
        UpdateIndicators();
    }
    private void RightLoop()
    {
        if ((CurrentIndex + 1) >= items.Count)
            CurrentIndex = 0;
        else
            CurrentIndex++;

        UpdateUI();
        AnimatorWork(true);
        UpdateIndicators();
    }

    private void LeftClamp()
    {
        if (CurrentIndex != 0)
        {

            if (CurrentIndex == 0)
                CurrentIndex = items.Count - 1;
            else
                CurrentIndex--;

            UpdateUI();
            AnimatorWork(false);
            UpdateIndicators();
        }
    }
    private void RightClamp()
    {
        if (CurrentIndex != items.Count - 1)
        {

            if ((CurrentIndex + 1) >= items.Count)
                CurrentIndex = 0;
            else
                CurrentIndex++;

            UpdateUI();
            AnimatorWork(true);
            UpdateIndicators();
        }
    }
    #endregion

    #region UpdateUI

    private void UpdateUI()
    {
        if (items.Count > 0)
        {
            ItemDataWrapper itemData = items[CurrentIndex].itemData;

            indicatorText.text = "1 of " + items[CurrentIndex].itemData.StackSize;//fix it;

            itemHelperText.text = itemText.text;
            itemText.text = itemData.scriptableData.objectName;

            itemIcon.sprite = itemData.scriptableData.itemSprite;
            itemIcon.enabled = true;

            rejectIcon.enabled = false;

            btnLeft.SetActive(true);
            btnRight.SetActive(true);
        }
        else
        {
            indicatorText.text = "";

            itemHelperText.text = "";
            itemText.text = "";

            itemIcon.sprite = null;
            itemIcon.enabled = false;

            rejectIcon.enabled = true;

            btnLeft.SetActive(false);
            btnRight.SetActive(false);
        }
    }

    private void AnimatorWork(bool forward)
    {
        animator.Play(null);
        animator.StopPlayback();

        if (forward)
        {
            if (isInverse)
            {
                animator.Play("Previous");
            }
            else
            {
                animator.Play("Forward");
            }
        }
        else
        {
            if (isInverse)
            {
                animator.Play("Forward");
            }
            else
            {
                animator.Play("Previous");
            }
        }
    }

    private void DisposeIndicators()
    {
        indicators.Clear();
        foreach (Transform child in indicatorsParent)
            Destroy(child.gameObject);
    }
    private void ReCreateIndicators()
    {
        DisposeIndicators();
        for (int i = 0; i < items.Count; ++i)
        {
            IndicatorUI indicator = Instantiate(indicatorPrefab, new Vector3(0, 0, 0), Quaternion.identity, indicatorsParent);

            indicator.gameObject.name = "_indicator_" + i;

            indicators.Add(indicator);
        }
    }
    private void UpdateIndicators()
    {
        for (int i = 0; i < indicators.Count; i++)
        {
            if (CurrentIndex == i)
                indicators[i].On();
            else
                indicators[i].Off();
        }
    }
    #endregion
}
