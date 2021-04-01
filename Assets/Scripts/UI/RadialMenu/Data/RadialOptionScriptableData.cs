using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "RadialOption", menuName = "RadialMenu/Option")]
public class RadialOptionScriptableData : ScriptableObject
{
    [PreviewField]
    public Sprite optionIcon;

    public virtual void EventInvoke() { }
}
[System.Serializable]
public class RadialOptionData
{
    public bool isEnd = false;
    public RadialOptionScriptableData scriptableData;
    public UnityEvent unityEvent; 

    public bool IsNull
    {
        get
        {
            for (int i = 0; i < unityEvent.GetPersistentEventCount(); i++)
            {
                if (unityEvent.GetPersistentTarget(i) != null)
                {
                    return false;
                }
            }
            return true;
        }
    }

    public void EventInvoke()
    {
        unityEvent?.Invoke();
        scriptableData?.EventInvoke();
    }
}
