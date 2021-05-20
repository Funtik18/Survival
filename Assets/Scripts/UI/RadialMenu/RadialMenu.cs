using Sirenix.OdinInspector;

using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

public class RadialMenu : WindowUI
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

    [SerializeField] private Pointer pointerOpen;
    [SerializeField] private Pointer pointerClose;

    [SerializeField] private Menus menus;

    private void Awake()
    {
        pointerOpen.AddPressListener(OpenRadialMenu);
        pointerClose.AddPressListener(CloseRadialMenu);

        menus.Setup();
    }

    public override void HideWindow()
    {
        base.HideWindow();

        CloseRadialMenu();
    }

    [Button]
    public void OpenRadialMenu()
    {
        ShowWindow();

        GeneralAvailability.Player.Lock();
        pointerClose.gameObject.SetActive(true);
        menus.menu[0].Open();//primary menu
    }
    [Button]
    private void CloseRadialMenu()
    {
        menus.Close();
        pointerClose.gameObject.SetActive(false);
        GeneralAvailability.Player.UnLock();
    }

    [System.Serializable]
    public class Menus 
    {
        [SerializeField] private Transform parent;
        [SerializeField] private PIRadialOption optionPrefab;

        private int maxOptions = 10;
        
        [ListDrawerSettings(ShowIndexLabels = true, CustomAddFunction = "OverrideAdd", CustomRemoveElementFunction = "OverrideRemove")]
        public List<Menu> menu = new List<Menu>();

        public void Setup()
        {
            for (int i = 0; i < menu.Count; i++)
            {
                menu[i].Setup();
            }
        }

        public void Close()
        {
            for (int i = 0; i < menu.Count; i++)
            {
                menu[i].Close();
            }
        }

        private void OverrideAdd()
        {
            CheckMenues();
            if (menu.Count < maxOptions)
            {
                PIRadialMenu radialMenu = new GameObject("Menu_" + menu.Count).AddComponent<PIRadialMenu>();
                radialMenu.transform.SetParent(parent);
                radialMenu.transform.localPosition = Vector3.zero;
                radialMenu.transform.localScale = Vector3.one;


                Menu newMenu = new Menu();
                newMenu.menu = radialMenu;
                newMenu.optionPrefab = optionPrefab;
                newMenu.maxOptions = maxOptions;

                menu.Add(newMenu);
            }
        }
        private void OverrideRemove(Menu element)
        {
            menu.Remove(element);
            if(element.menu != null)
                DestroyImmediate(element.menu.gameObject);

            CheckMenues();
        }

        [Button]
        private void UpdateMenu()
        {
            for (int i = 0; i < menu.Count; i++)
            {
                menu[i].UpdateMenu();
            }
        }
        
        private void CheckMenues()
        {
            for (int i = 0; i < menu.Count; i++)
            {
                if (menu[i] == null)
                    menu.RemoveAt(i);
            }

            for (int i = 0; i < parent.childCount; i++)
            {
                parent.GetChild(i).gameObject.name = "Menu_" + i;
            }

            for (int i = 0; i < menu.Count; i++)
            {
                menu[i].Refresh();
            }
        }
    }
    [System.Serializable]
    public class Menu 
    {
        [HideInInspector] public PIRadialMenu menu;
        [OnValueChanged("RenameMenu")]
        [SerializeField] private string customMenuName;

        [HideInInspector] public PIRadialOption optionPrefab;
        [HideInInspector] public int maxOptions;

        [ListDrawerSettings(ShowIndexLabels = true, CustomAddFunction = "OverrideAdd", CustomRemoveElementFunction = "OverrideRemove")]
        public List<Option> options = new List<Option>();

        private Transform Parent => menu.transform;

        public void Setup()
        {
            for (int i = 0; i < options.Count; i++)
            {
                options[i].Setup(menu);
            }
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

        public void UpdateMenu()
        {
            for (int i = 0; i < options.Count; i++)
            {
                options[i].option.transform.SetSiblingIndex(i);
            }

            menu.UpdateMenu();
            CheckOptions();
        }


        private void OverrideAdd()
        {
            CheckOptions();

            if (options.Count < maxOptions)
            {
                PIRadialOption radialOption = Instantiate(optionPrefab, Parent);
                radialOption.gameObject.name = "Option_" + options.Count;

                Option option = new Option();
                option.option = radialOption;

                options.Add(option);
            }

            menu.UpdateMenu();
        }
        private void OverrideRemove(Option element)
        {
            options.Remove(element);
            
            if(element.option != null)
                DestroyImmediate(element.option.gameObject);

            CheckOptions();
        
            menu.UpdateMenu();
        }

        public void Refresh()
        {
            if(customMenuName != "")
            {
                RenameMenu();
            }
        }

        private void RenameMenu()
        {
            menu.gameObject.name = customMenuName;
        }

        private void CheckOptions()
        {
            for (int i = 0; i < options.Count; i++)
            {
                if (options[i] == null)
                    options.RemoveAt(i);
            }

            for (int i = 0; i < Parent.childCount; i++)
            {
                Parent.GetChild(i).gameObject.name = "Option_" + i;
            }
        }
    }
    [System.Serializable]
    public class Option
    {
        [HideInInspector] public PIRadialOption option;

        [SerializeField] private PIRadialMenu connection;

        [SerializeField] private bool isBuilding = false;

        [HideIf("isBuilding")]
        [OnValueChanged("IconChanged")]
        [SerializeField] private Sprite icon;

        [ShowIf("isBuilding")]
        [OnValueChanged("IconChanged")]
        [SerializeField] private BuildingSD building;

        [SerializeField] private UnityEvent unityEvent;

        private bool isEnd = true;
        public bool IsEnd => isEnd;

        public bool IsWithNulls
        {
            get
            {
                for (int i = 0; i < unityEvent.GetPersistentEventCount(); i++)
                {
                    if (unityEvent.GetPersistentTarget(i) == null)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        public bool IsEmpty => unityEvent.GetPersistentEventCount() == 0;

        private PIRadialMenu owner;

        public void Setup(PIRadialMenu owner)
        {
            this.owner = owner;
            owner.onOpened += Check;
            owner.onOpened += IconChanged;

            option.onChoosen = EventInvoke;
        }

        private void Check()
        {
            if (isBuilding)
            {
                if(building != null)
                {
                    option.IsProhibition = !GeneralAvailability.Player.Build.IsCanBuild(building);
                }
                else
                {
                    option.IsProhibition = true;
                }
            }

            isEnd = connection == null;
        }

        private void EventInvoke()
        {
            if (IsEnd)
            {
                RadialMenu.Instance.CloseRadialMenu();
            }
            else
            {
                owner.CloseMenu();
            }

            unityEvent?.Invoke();

            if(isBuilding)
                if(GeneralAvailability.Player.Build.IsCanBuild(building))
                    GeneralAvailability.Build.BuildBuilding(building.model);

            if (!IsEnd)
                connection.OpenMenu();
        }

        private void IconChanged()
        {
            option.Setup(isBuilding ? building?.buildingSprite : icon);
        }
    }
}