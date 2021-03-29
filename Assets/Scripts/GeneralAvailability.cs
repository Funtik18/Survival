using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralAvailability : MonoBehaviour
{
    private static Player player;
    public static Player Player
    {
        get
        {
            if (player == null)
            {
                player = Player.Instance;
            }
            return player;
        }
    }

    private static Inventory playerInventory;
    public static Inventory PlayerInventory
    {
        get
        {
            if (playerInventory == null)
            {
                playerInventory = Player.Inventory;
            }
            return playerInventory;
        }
    }


    private static ItemInspector inspector;
    public static ItemInspector Inspector
    {
        get
        {
            if (inspector == null)
                inspector = Player.itemInspector;
            return inspector;
        }
    }


    private static WindowItemInspector inspectorWindow;
    public static WindowItemInspector InspectorWindow
    {
        get
        {
            if (inspectorWindow == null)
                inspectorWindow = Player.playerUI.windowsUI.itemInspectorWindow;
            return inspectorWindow;
        }
    }

    private static WindowBackpack backpackWindow;
    public static WindowBackpack BackpackWindow
    {
        get
        {
            if (backpackWindow == null)
                backpackWindow = Player.playerUI.windowsUI.backpackWindow;
            return backpackWindow;
        }
    }


    private static TargetPoint targetPoint;
    public static TargetPoint TargetPoint
    {
        get
        {
            if (targetPoint == null)
                targetPoint = Player.playerUI.controlUI.targetPoint;
            return targetPoint;
        }
    }

    private static HoldLoader loader;
    public static HoldLoader Loader
    {
        get
        {
            if (loader == null)
                loader = TargetPoint.holdLoader;
            return loader;
        }
    }



    private static FixedTouchButton buttonPickUp;
    public static FixedTouchButton ButtonPickUp
    {
        get
        {
            if (buttonPickUp == null)
                buttonPickUp = Player.playerUI.controlUI.buttonPickUp;
            return buttonPickUp;
        }
    }

    private static FixedTouchButton buttonSearch;
    public static FixedTouchButton ButtonSearch
    {
        get
        {
            if (buttonSearch == null)
                buttonSearch = Player.playerUI.controlUI.buttonSearch;
            return buttonSearch;
        }
    }
}
