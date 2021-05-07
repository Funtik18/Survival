using UnityEngine;
using UnityEngine.Events;

public class WindowIgnition : WindowUI
{
    public UnityAction onBack;
    public UnityAction onStart;

    [SerializeField] private Pointer background;
    [SerializeField] private CustomPointer buttonBack;
    [SerializeField] private CustomPointer buttonHelp;

    [SerializeField] private IgnitionRequirementsUI requirements;
    [Space]
    [SerializeField] private TMPro.TextMeshProUGUI baseChanceText;
    [SerializeField] private TMPro.TextMeshProUGUI estimatedFireText;
    [SerializeField] private TMPro.TextMeshProUGUI estimatedFireDurationText;
    [SerializeField] private TMPro.TextMeshProUGUI chanceSuccessText;

    private void Awake()
    {
        buttonBack.pointer.onPressed.AddListener(Back);

        background.AddDoublePressListener(StartIgnition);
    }

    public void OpenWindow()
    {
        ShowWindow();
    }

    public void Setup(RequirementsIgnition requirementsValues)
    {
        requirements.Setup(requirementsValues);
    }

    public void UpdateUI(float baseChance, float successChance, string estimatedFire, string estimatedFireDuration)
    {
        baseChanceText.text = baseChance + "%";
        estimatedFireText.text = estimatedFire;
        estimatedFireDurationText.text = estimatedFireDuration;
        chanceSuccessText.text = successChance + "%";
    }

    private void StartIgnition()
    {
        onStart?.Invoke();
    }
    public void Back()
    {
        onBack?.Invoke();
    }
}