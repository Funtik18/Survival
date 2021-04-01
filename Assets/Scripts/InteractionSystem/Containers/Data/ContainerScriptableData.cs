using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Container", menuName = "Environment/Container")]
public class ContainerScriptableData : ObjectScriptableData
{
    [SuffixLabel("s", Overlay = true)]
    [Min(1f)]
    public float time = 1f;
}