using UnityEngine;

using Sirenix.OdinInspector;

public abstract class ConsumableItemSD : ItemSD 
{
    public bool isCookable= false;

    [Tooltip("Максимально возможное количество калорий.")]
    [SuffixLabel("Kcal")]
    [Range(0, 2500)]
    public float calories = 0f;

    [Tooltip("Зависит от каллорий, сколько процентов жажды утолит при 100% каллорий.")]
    [SuffixLabel("%", true)]
    [Range(-100f, 100f)]
    public float hydration = 0f;

    [TabGroup("RANDOM")]
    public bool isCanRandomCalories = false;
}