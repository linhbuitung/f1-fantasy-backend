using F1FantasyWorker.Core.Common;

namespace F1FantasyWorker.Modules.CoreGameplayModule.Helpers;

public class PointCalculationHelper
{
    public static int GetDriverQualificationPoint(int? position)
    {
        return position switch
        {
            1 => 10,
            2 => 9,
            3 => 8,
            4 => 7,
            5 => 6,
            6 => 5,
            7 => 4,
            8 => 3,
            9 => 2,
            10 => 1,
            _ => 0
        };
    }

    public static int GetDriversQualificationPoint(List<RaceEntry> raceEntries)
    {
        int totalPoints = 0;
        foreach (var raceEntry in raceEntries)
        {
            if (raceEntry.Grid != null)
                totalPoints += GetDriverQualificationPoint(raceEntry.Grid ?? -1);
        }
        return totalPoints;
    }
    
    public static int GetDriverRacePoint(int? position, bool isFinished)
    {
        if ( !isFinished ) return -25;
        
        return position switch
        {
            1 => 25,
            2 => 18,
            3 => 15,
            4 => 12,
            5 => 10,
            6 => 8,
            7 => 6,
            8 => 4,
            9 => 2,
            10 => 1,
            _ => 0
        };
    }

    public static int GetDriversRacePoint(List<RaceEntry> raceEntries)
    {
        int totalPoints = 0;
        foreach (var raceEntry in raceEntries)
        {
            if (raceEntry.Position != null)
                totalPoints += GetDriverQualificationPoint(raceEntry.Position ?? -1);
        }
        return totalPoints;
    }
    /*
     
    Neither driver reaches top 10 
    -1 point 

    One driver reaches top 10 
    1 point 

    Both drivers reach top 10 
    3 points 

    One driver reaches top 3 
    5 points 

    Both drivers reach top 3 
    10 points 
     */
    public static int GetConstructorQualificationPoint(
        int driverOnePosition,
        int driverTwoPosition)
    {
        int totalPoints = 0;

        if (driverOnePosition > 10 && driverTwoPosition > 10)
            totalPoints = -1;
        
        if(driverOnePosition <= 10 && driverTwoPosition <= 10)
            totalPoints = 3;
        
        if(driverOnePosition <= 10 || driverTwoPosition <= 10)
            totalPoints = 1;

        if (driverOnePosition <= 3 || driverTwoPosition <= 3)
            totalPoints = 5;
        
        if (driverOnePosition <= 3 && driverTwoPosition <= 3)
            totalPoints = 10;
        
        return totalPoints;
    }

    public static int GetDriverPositionGainPoint(int? startPosition, int? endPosition)
    {
        if(startPosition == null || endPosition == null)
            return 0;
        return endPosition.Value - startPosition.Value;
    }
    
    public static int GetDriverFastestLapPoint(int? fastestLap)
    {
        if (fastestLap != null && fastestLap == 1)
            return 3;
        return 0;
    }
}