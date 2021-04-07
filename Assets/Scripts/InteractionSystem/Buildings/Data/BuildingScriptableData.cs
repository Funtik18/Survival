using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;
using System;
using System.Linq;
using Sirenix.Utilities;

[CreateAssetMenu(fileName = "BuildingScriptableData", menuName = "Buildings/Building")]
public class BuildingScriptableData : ObjectScriptableData
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