using UnityEngine;

using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "BuildingSD", menuName = "Buildings/Building")]
public class BuildingSD : ObjectSD
{
    [AssetList]
    [InlineEditor(InlineEditorModes.GUIAndPreview)]
    public BuildingObject model;

    [Tooltip("Если тру значит сразу вычитает нужные для рецепта ингредиенты, или это всё делается в ui.")]
    public bool isImmediatelyCalculation = false;

    [Tooltip("Если тру значит рецепт составлен в коде.")]
    public bool isComplex = false;

    [HideIf("isComplex")]
    [Space]
    public BuildingRecipt recipt;
}