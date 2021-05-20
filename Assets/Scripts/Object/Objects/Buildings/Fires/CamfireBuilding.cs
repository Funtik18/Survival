public class CamfireBuilding : FireBuilding
{
    public override void Place()
    {
        base.Place();

        OpenIgnitionWindow();
    }
}