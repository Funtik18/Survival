using UnityEngine;

using Sirenix.OdinInspector;

public abstract class FireSD : ItemSD
{
    [Title("Benefit")]
    [SuffixLabel("%", true)]
    [Range(-100f, 100f)]
    [Tooltip("К шансу розжига.")]
    public float chance = 0f;
}