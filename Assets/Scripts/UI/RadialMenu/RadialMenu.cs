using Sirenix.OdinInspector;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RadialMenu : MonoBehaviour
{
    [SerializeField] private PIRadialMenu primaryMenu;

    [ListDrawerSettings(HideAddButton = true, HideRemoveButton = true, ShowIndexLabels = true)]
    public RadialOptionData[] options = new RadialOptionData[10];

    private void Awake()
    {
        primaryMenu.Setup(options);
    }



    [Button]
    public void OpenRadialMenu()
    {
        primaryMenu.OpenMenu();
    }
    [Button]
    public void CloseRadialMenu()
    {
        primaryMenu.CloseMenu();
    }
}
[System.Serializable]
public class RadialOptionData
{
    [PreviewField(Alignment = ObjectFieldAlignment.Left)]
    public Sprite optionIcon;
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
    }
}