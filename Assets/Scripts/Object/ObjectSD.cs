using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectSD : ScriptableObject
{
    public string objectName;
    [TextArea(5, 5)]
    public string description;
}