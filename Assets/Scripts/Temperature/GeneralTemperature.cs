using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

public class GeneralTemperature : MonoBehaviour
{
    private static GeneralTemperature instance;
    public static GeneralTemperature Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GeneralTemperature>();
            }
            return instance;
        }
    }

    [Range(-100f, 100f)]
    public float airResistance = 0f;
    [Range(-100f, 100f)]
    public float windResistance = 0f;
    [Range(0, 100f)]
    public float clothing = 0f;
    [Space]
    [Range(-100f, 50f)]
    public float airTemperature = 0f;
    [Range(-100f, 0)]
    public float windchill = 0f;
    
    public Wind windType = Wind.Calm;
    public Vector3 windDirection = Vector3.zero;
    public float windSpeed;

    [ShowInInspector]
    public float AirFeels => Mathf.Clamp(airTemperature - (airTemperature * airResistance / 100f), -100f, 50f);
    [ShowInInspector]
    public float WindFeels => Mathf.Min(windchill - (windchill * windResistance / 100f), 0);


    //если > 0 получаем тепло
    //если <= 0 теряет тепло
    [ShowInInspector]
    public float FeelsLike
    { 
        get => AirFeels + WindFeels + clothing;//+ bonuses
    }
}
public enum Wind 
{
    Calm,//спокойный
    Light,//легкий
    Moderate,//умеренный модифицируемый
    High,//сильный

    //Calm: no wind, Windchill, or penalties.
    //Light-Moderate: Windchill is present.
    //High: slows movement speed, lighting exposed fires is impossible.
}

public enum WeatherType
{
    Clear,
    Aurora,
    Cloudy,
    Fog,
    Snowfall,
    Blizzard,
}
