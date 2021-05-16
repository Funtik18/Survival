using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralAvailability : MonoBehaviour
{
    //Player
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

    private static PlayerUI playerUI;
    public static PlayerUI PlayerUI
    {
        get
        {
            if (playerUI == null)
            {
                playerUI = Player.UI;
            }
            return playerUI;
        }
    }


    private static PlayerInventory playerInventory;
    public static PlayerInventory PlayerInventory
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

    //Build
    private static Build build;
    public static Build Build
    {
        get
        {
            if(build == null)
            {
                build = Player.Build;
            }
            return build;
        }
    }

    //Inspector
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

    private static WindowBackpack backpackWindow;
    public static WindowBackpack BackpackWindow
    {
        get
        {
            if (backpackWindow == null)
                backpackWindow = Player.UI.windowsUI.backpackWindow;
            return backpackWindow;
        }
    }

    private static WindowExchanger exchangerWindow;
    public static WindowExchanger ExchangerWindow
    {
        get
        {
            if (exchangerWindow == null)
                exchangerWindow = Player.UI.windowsUI.exchangerWindow;
            return exchangerWindow;
        }
    }


    private static TargetPoint targetPoint;
    public static TargetPoint TargetPoint
    {
        get
        {
            if (targetPoint == null)
                targetPoint = Player.UI.targetPoint;
            return targetPoint;
        }
    }

    private static InteractionButton buttonInteraction;
    public static InteractionButton ButtonInteraction
    {
        get
        {
            if (buttonInteraction == null)
                buttonInteraction = Player.UI.controlUI.buttonInteraction;
            return buttonInteraction;
        }
    }
}
