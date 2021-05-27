using UnityEngine;

using Sirenix.OdinInspector;

public abstract class FireItemSD : ItemSD
{
    [Title("Benefit")]
    [SuffixLabel("%", true)]
    [Range(-100f, 100f)]
    [Tooltip("К шансу розжига.")]
    public float chance = 0f;
}