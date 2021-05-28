using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HarvestingToolUI : MonoBehaviour
{
    public UnityAction onLeft;
    public UnityAction onRight;

    [SerializeField] private Image toolIcon;
    [SerializeField] private Sprite hands;
    [SerializeField] private Color handsColor;
    [SerializeField] private TMPro.TextMeshProUGUI toolName;
    [SerializeField] private Pointer buttonToolLeft;
    [SerializeField] private Pointer buttonToolRight;

    private ToolItemSD tool;

    private bool isFirstTime = true;

    public void SetTool(ToolItemSD tool)
    {
        this.tool = tool;

        if (isFirstTime)
        {
            Setup();
            isFirstTime = false;
        }

        UpdateUI();
    }

    public void EnableLeft(bool trigger)
    {
        buttonToolLeft.gameObject.SetActive(trigger);
    }
    public void EnableRight(bool trigger)
    {
        buttonToolRight.gameObject.SetActive(trigger);
    }


    private void Setup()
    {
        buttonToolLeft.AddPressListener(Left);
        buttonToolRight.AddPressListener(Right);
    }
    private void UpdateUI()
    {
        if(tool == null)
        {
            toolIcon.color = handsColor;
            toolIcon.sprite = hands;
            toolName.text = "hands";
        }
        else
        {
            toolIcon.color = Color.white;
            toolIcon.sprite = tool.itemSprite;
            toolName.text = tool.objectName;
        }
    }
    private void Left()
    {
        onLeft?.Invoke();
    }
    private void Right()
    {
        onRight?.Invoke();
    }
}