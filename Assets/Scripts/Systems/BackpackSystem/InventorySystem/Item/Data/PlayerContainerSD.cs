using Sirenix.OdinInspector;

using UnityEngine;

[CreateAssetMenu(menuName = "Game/PlayerContainer", fileName = "Data")]
public class PlayerContainerSD : ScriptableObject
{
    [HideLabel]
    public Container container;
}
