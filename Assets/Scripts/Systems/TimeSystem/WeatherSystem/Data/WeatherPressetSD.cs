using UnityEngine;

[CreateAssetMenu(menuName = "Game/Environment/WeatherPresset", fileName = "Data")]
public class WeatherPressetSD : ScriptableObject
{
    public WeatherType weatherType;
    public FogPressetSD fog;
}
