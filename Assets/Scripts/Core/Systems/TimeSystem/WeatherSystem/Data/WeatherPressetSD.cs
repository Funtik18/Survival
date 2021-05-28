using Sirenix.OdinInspector;

using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(menuName = "Game/Environment/WeatherPresset", fileName = "Data")]
public class WeatherPressetSD : ScriptableObject
{

    public bool randomParticle;
    [ShowIf("randomParticle")]
    public List<ParticleSystem> particles = new List<ParticleSystem>();
    [InfoBox("May be null")]
    [HideIf("randomParticle")]
    public ParticleSystem particle;


    public bool randomFog;
    [InfoBox("May be null")]
    [ShowIf("randomFog")]
    public List<FogPressetSD> fogs = new List<FogPressetSD>();
    [InfoBox("May be null")]
    [HideIf("randomFog")]
    public FogPressetSD fog;
}
