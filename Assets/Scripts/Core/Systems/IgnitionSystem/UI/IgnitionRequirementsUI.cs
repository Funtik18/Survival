using UnityEngine;

public class IgnitionRequirementsUI : MonoBehaviour
{
    [SerializeField] private IgnitionRequirementUI requirementStarter;
    [SerializeField] private IgnitionRequirementUI requirementTinder;
    [SerializeField] private IgnitionRequirementUI requirementFuel;
    [SerializeField] private IgnitionRequirementUI requirementAccelerant;

    private RequirementsIgnition requirements;

    public void Setup(RequirementsIgnition requirements)
    {
        this.requirements = requirements;

        UpdateUI();
    }

    private void UpdateUI()
    {
        requirementStarter.Setup(requirements.starters);
        requirementTinder.Setup(requirements.tinders);
        requirementFuel.Setup(requirements.fuels);
        requirementAccelerant.Setup(requirements.accelerants);

        if (requirements.accelerants.requirements.Count == 0)
            requirementAccelerant.HideWindow();
        else
            requirementAccelerant.ShowWindow();
    }
}