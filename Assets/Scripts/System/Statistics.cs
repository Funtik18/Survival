public static class Statistics
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

    public static int TotalDaysSurvived;


    public static int TotalContainersInspected;//счёт всего открывания контейнеров(первый раз)
    public static int TotalContainersOpened;//счёт всего открывания контейнеров

    public static void Init()
    {
        GeneralTime.Instance.onSecond += SecondFormules;
        GeneralTime.Instance.onDay += DayFormules;
    }

    private static void SecondFormules()
    {
        TotalSecondSurvived++;
    }
    private static void DayFormules()
    {
        TotalDaysSurvived++;
    }


    public static void SetData(Data data)
    {
        TotalSecondSurvived = data.totalSecondSurvived;
        TotalDaysSurvived = data.totalDaysSurvived;

        TotalContainersInspected = data.totalContainersInspected;
        TotalContainersOpened = data.totalContainersOpened;
    }

    public static Data GetData()
    {
        Data statistic = new Data()
        {
            totalSecondSurvived = TotalSecondSurvived,
            totalDaysSurvived = TotalDaysSurvived,

            totalContainersInspected = TotalContainersInspected,
            totalContainersOpened = TotalContainersOpened,
        };

        return statistic;
    }

    [System.Serializable]
    public class Data
    {
        public int totalSecondSurvived;
        public int totalDaysSurvived;

        public int totalContainersInspected;
        public int totalContainersOpened;
    }
}