using UnityEngine;

using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "Environment/Board", fileName ="Board")]
public class BoardScriptableData : ScriptableObject
{
    [HideLabel]
    public BoardData data;
}
[System.Serializable]
public class BoardData
{
    [TextArea(5, 5)]
    public string description;
}
