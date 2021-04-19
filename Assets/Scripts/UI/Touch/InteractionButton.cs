using UnityEngine;
using UnityEngine.UI;

using Sirenix.OdinInspector;

public class InteractionButton : CustomPointer
{
    [SerializeField] private Image icon;

    [SerializeField] private Sprite interaction;
    [SerializeField] private Sprite search;
    [SerializeField] private Sprite pickUp;

    [Button]
    public void SetIconOnInteraction()
    {
        icon.sprite = interaction;
    }
    public void SetIconOnSearch()
    {
        icon.sprite = search;
    }
    [Button]
    public void SetIconOnPickUp()
    {
        icon.sprite = pickUp;
    }
}
