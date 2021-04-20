using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class SceneGenerator
{
    static SceneGenerator()
    {
        if (ItemsData.Instance == null)
            Debug.LogError("Items == null");
    }
}