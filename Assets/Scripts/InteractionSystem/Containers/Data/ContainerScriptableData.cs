using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Container", menuName = "Environment/Container")]
public class ContainerScriptableData : ScriptableObject
{
    [HideLabel]
    public ContainerData data;
}
[System.Serializable]
public class ContainerData
{
    public string name;
    [SuffixLabel("s", Overlay = true)]
    [Min(1f)]
    public float openTime = 1f;
}