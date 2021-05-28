public static class GeneralStatistics
{
    public static int TotalSecondSurvived = 0;
    private static Times timesSurvived;
    public static Times TimesSurvived 
    {
        get
        {
            timesSurvived.TotalSeconds = TotalSecondSurvived;
            timesSurvived.CheckTimeSeconds();

            return timesSurvived;
        }
    }

    public static int DaysSurvived;

    public static void Init()
    {
        GeneralTime.Instance.onSecond += SecondFormules;
        GeneralTime.Instance.onDay += DayFormules;
    }
    public static void SetData(Statistic data)
    {
        TotalSecondSurvived = data.totalSecondSurvived;
    }

    private static void SecondFormules()
    {
        TotalSecondSurvived++;
    }

    private static void DayFormules()
    {

    }

    public static Statistic GetData()
    {
        Statistic statistic = new Statistic()
        {
            totalSecondSurvived = TotalSecondSurvived,
        };

        return statistic;
    }
}
public struct Statistic 
{
    public int totalSecondSurvived;
}

