using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsLaws : MonoBehaviour
{
    private static ItemsLaws instance;
    public static ItemsLaws Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ItemsLaws>();

                if (instance == null)
                {
                    instance = new GameObject("_ItemsLaws").AddComponent<ItemsLaws>();
                }
                if (Application.isPlaying)
                    DontDestroyOnLoad(instance);
            }
            return instance;
        }
    }

    [Tooltip("За сколько растает 1кг снега.")]
    public Times meltTime;
    [Tooltip("За сколько вскипит 1L жидкость.")]
    public Times boilsTime;
    [Tooltip("За сколько жидкость испарится.")]
    public Times evaporationTime;
}
