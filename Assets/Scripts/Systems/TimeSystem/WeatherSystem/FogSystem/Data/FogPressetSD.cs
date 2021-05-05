using UnityEngine;

[CreateAssetMenu(menuName = "Game/Environment/FogPresset", fileName = "Data")]
public class FogPressetSD : ScriptableObject
{
    public Color fogColor;
    [Range(0, 1f)] 
    public float rangeValue;
}