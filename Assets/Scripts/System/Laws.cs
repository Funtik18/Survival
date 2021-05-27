using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laws : MonoBehaviour
{
    private static Laws instance;
    public static Laws Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<Laws>();

                if (instance == null)
                {
                    instance = new GameObject("_Laws").AddComponent<Laws>();
                }
                if (Application.isPlaying)
                    DontDestroyOnLoad(instance);
            }
            return instance;
        }
    }

    [Tooltip("Максимальное колличество времени для сна.")]
    public Times maxSleepTime;
    [Tooltip("Максимальное колличество секунд в реальном времения пройдёт на весь отдых.")]
    public float waitRealTimeSleeping;

    [Tooltip("Максимальное колличество времени для пропуска времени.")]
    public Times maxPassTime;
    [Tooltip("Максимальное колличество секунд в реальном времения пройдёт на весь пропуск времени.")]
    public float waitRealTimePassing;

    [Tooltip("Сколько реального времени уйдёт на 1 час кравта.")]
    public float waitRealTimeCraft = 5f;

    [Tooltip("Сколько реального времени уйдёт на 1 час ожидания pass time.")]
    public float waitRealTimePassTime = 5f;//need curve

    [Tooltip("За сколько растает 1кг снега.")]
    public Times meltTime;
    [Tooltip("За сколько вскипит 1L жидкость.")]
    public Times boilsTime;
    [Tooltip("За сколько жидкость испарится.")]
    public Times evaporationTime;
}
