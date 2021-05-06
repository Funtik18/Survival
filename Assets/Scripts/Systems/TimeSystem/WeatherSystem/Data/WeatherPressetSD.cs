using Sirenix.OdinInspector;

using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(menuName = "Game/Environment/WeatherPresset", fileName = "Data")]
public class WeatherPressetSD : ScriptableObject
{
    //public WeatherType weatherType;


    public bool randomParticle;
    [ShowIf("randomParticle")]
    public List<ParticleSystem> particles = new List<ParticleSystem>();
    [HideIf("randomParticle")]
    public ParticleSystem particle;


    public bool randomFog;
    [ShowIf("randomFog")]
    public List<FogPressetSD> fogs = new List<FogPressetSD>();
    [HideIf("randomFog")]
    public FogPressetSD fog;
}
