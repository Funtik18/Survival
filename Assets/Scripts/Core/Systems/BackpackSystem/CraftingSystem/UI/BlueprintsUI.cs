using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

public class BlueprintsUI : MonoBehaviour
{
    public UnityAction<BlueprintAvailability> onSelected;

    [SerializeField] private Transform root;
    [SerializeField] private BlueprintUI blueprintPrefab;

    private List<BlueprintUI> blueprintsUI = new List<BlueprintUI>();

    private List<BlueprintAvailability> blueprintAvailabilities;

    private BlueprintUI selected;

    public void Setup(List<BlueprintAvailability> blueprintAvailabilities)
    {
        this.blueprintAvailabilities = blueprintAvailabilities;

        ReCreate();

        for (int i = 0; i < blueprintAvailabilities.Count; i++)
        {
            blueprintsUI[i].Setup(blueprintAvailabilities[i]);
        }
    }

    public void Select(BlueprintSD blueprint)
    {
        for (int i = 0; i < blueprintsUI.Count; i++)
        {

            if (blueprintsUI[i].Is(blueprint))
            {
                blueprintsUI[i].Select();
            }
            else
            {
                blueprintsUI[i].Deselect();
            }
        }

    }

    public void Clear()
    {
        if (selected)
        {
            selected.Deselect();
            selected = null;
        }
    }
    
    public void UpdateUI()
    {
        for (int i = 0; i < blueprintAvailabilities.Count; i++)
        {
            blueprintsUI[i].UpdateUI();
        }
    }

    private void ReCreate()
    {
        Dispose();
        Create();
    }
    private void Create()
    {
        for (int i = 0; i < blueprintAvailabilities.Count; i++)
        {
            BlueprintUI blueprint = Instantiate(blueprintPrefab, root);
            blueprint.onSelected += SelectedBlueprint;
            blueprint.onSelectedBlueprint += SelectedBlueprint;
            blueprintsUI.Add(blueprint);
        }
    }
    private void Dispose()
    {
        for (int i = root.childCount - 1; i >= 0 ; i--)
        {
            DestroyImmediate(root.GetChild(i).gameObject);
        }

        blueprintsUI.Clear();
    }

    private void SelectedBlueprint(BlueprintUI blueprint)
    {
        selected = blueprint;

        for (int i = 0; i < blueprintsUI.Count; i++)
        {
            if (blueprintsUI[i] != selected)
            {
                blueprintsUI[i].Deselect();
            }
        }
    }
    private void SelectedBlueprint(BlueprintAvailability blueprint)
    {
        onSelected?.Invoke(blueprint);
    }
}