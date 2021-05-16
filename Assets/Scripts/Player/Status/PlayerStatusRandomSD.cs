using Sirenix.OdinInspector;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Player/PlayerStatusPresset", fileName = "Data")]
public class PlayerStatusRandomSD : ScriptableObject
{
    [InfoBox("RandomBetween")]

    [MinMaxSlider(10, 100)]
    public Vector2Int condition;

    [MinMaxSlider(0, 100)]
    public Vector2Int stamina;

    [MinMaxSlider(0, 100)]
    public Vector2Int warmth;

    [MinMaxSlider(0, 100)]
    public Vector2Int fatigue;

    [MinMaxSlider(0, 2500)]
    public Vector2Int hunger;

    [MinMaxSlider(0, 0.67f)]
    public Vector2 thirst;


    public PlayerStatusData GetData()
    {
        PlayerStatusData data = new PlayerStatusData()
        {
            statsData = new PlayerStatsData()
            {
                condition = new StatBarData() { currentValue = condition.RandomNumBtw(), maxValue = 100 },
                stamina = new StatBarData() { currentValue = stamina.RandomNumBtw(), maxValue = 100 },
                warmth = new StatBarData() { currentValue = warmth.RandomNumBtw(), maxValue = 100 },
                fatigue = new StatBarData() { currentValue = fatigue.RandomNumBtw(), maxValue = 100 },
                hunger = new StatBarData() { currentValue = hunger.RandomNumBtw(), maxValue = 2500 },
                thirst = new StatBarData() { currentValue = thirst.RandomNumBtw(), maxValue = 0.67f },
            }
        };

        return data;
    }
}
