using UnityEngine;

using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "Game/Container", fileName = "Data")]
public class ContainerSD : ObjectSD
{
    [SuffixLabel("s", Overlay = true)]
    [Min(1f)]
    public float time = 1f;
}