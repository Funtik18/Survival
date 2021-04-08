using UnityEngine;

using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Container", menuName = "Environment/Container")]
public class ContainerSD : ObjectSD
{
    [SuffixLabel("s", Overlay = true)]
    [Min(1f)]
    public float time = 1f;
}