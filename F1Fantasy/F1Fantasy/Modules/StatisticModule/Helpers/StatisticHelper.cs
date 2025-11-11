namespace F1Fantasy.Modules.StatisticModule.Helpers;

public class StatisticHelper
{
    public static int CalculateConstructorPoints(List<int?> driverPositions)
    {
        if (driverPositions.Count == 2)
        {
            bool bothTop3 = driverPositions.All(p => p > 0 && p <= 3);
            bool oneTop3 = driverPositions.Count(p => p > 0 && p <= 3) == 1;
            bool bothTop10 = driverPositions.All(p => p > 0 && p <= 10);
            bool oneTop10 = driverPositions.Count(p => p > 0 && p <= 10) == 1;

            if (bothTop3)
                return 30;
            if (oneTop3)
                return 20;
            if (bothTop10)
                return 15;
            if (oneTop10)
                return 10;
        }
        return -10;
    }
}