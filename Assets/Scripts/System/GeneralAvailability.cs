public class GeneralAvailability
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
}