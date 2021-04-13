using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

public class RandomContainer : MonoBehaviour
{
    [SerializeField] private bool useRandomCount = false;
    [ShowIf("useRandomCount")]
    [MinMaxSlider(1, 10)]
    [SerializeField] private Vector2Int randomCountBtw;
    [HideIf("useRandomCount")]
    [Min(0)]
    [SerializeField] private int randomCount;


    private void Awake()
    {
        //ItemsSD.Instance
    }
}