using Sirenix.OdinInspector;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RadialMenu : MonoBehaviour
{
    private static RadialMenu instance;
    public static RadialMenu Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<RadialMenu>();
            }
            return instance;
        }
    }

    [SerializeField] private Menus menus;

    [SerializeField] private CustomButton buttonOpen;
    [SerializeField] private CustomButton buttonClose;

    private void Awake()
    {
        menus.Setup();

        buttonOpen.pointer.AddPressListener(OpenRadialMenu);
        buttonClose.pointer.AddPressListener(CloseRadialMenu);
    }

    [Button]
    public void OpenRadialMenu()
    {
        GeneralAvailability.Player.Lock();
        buttonClose.OpenButton();
        menus.menues[0].Open();//primary menu
    }
    [Button]
    public void CloseRadialMenu()
    {
        menus.Close();
        buttonClose.CloseButton();
        GeneralAvailability.Player.UnLock();
    }

    [System.Serializable]
    public class Menus 
    {
        public List<Menu> menues = new List<Menu>();

        [Button]
        public void Setup()
        {
            for (int i = 0; i < menues.Count; i++)
            {
                menues[i].Setup();
            }
        }

        public void Close()
        {
            for (int i = 0; i < menues.Count; i++)
            {
                menues[i].Close();
            }
        }
    }
    [System.Serializable]
    public class Menu 
    {
        public PIRadialMenu menu;
        [ListDrawerSettings(HideAddButton = true, ShowIndexLabels = true)]
        public List<RadialOptionData> options = new List<RadialOptionData>();

        public void Setup()
        {
            menu.Setup(options);
        }

        [Button]
        private void AddOption()
        {
            options.Add(null);
        }


        [Button]
        public void Open()
        {
            menu.OpenMenu();
        }
        [Button]
        public void Close()
        {
            menu.CloseMenu();
        }
    }
}
