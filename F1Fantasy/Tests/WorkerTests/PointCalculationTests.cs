namespace Tests.WorkerTests;
using System.Collections.Generic;
using Xunit;
using F1FantasyWorker.Modules.CoreGameplayModule.Helpers;
using F1FantasyWorker.Core.Common;

public class PointCalculationTests
{
    [Theory]
    [InlineData(1, 10)]
    [InlineData(2, 9)]
    [InlineData(3, 8)]
    [InlineData(4, 7)]
    [InlineData(5, 6)]
    [InlineData(6, 5)]
    [InlineData(7, 4)]
    [InlineData(8, 3)]
    [InlineData(9, 2)]
    [InlineData(10, 1)]
    [InlineData(11, 0)]
    [InlineData(null, 0)]
    public void GetDriverQualificationPoint_ReturnsExpected(int? pos, int expected)
    {
        var actual = PointCalculationHelper.GetDriverQualificationPoint(pos);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GetDriversQualificationPoint_SumsGridPoints_SkipsNulls()
    {
        var entries = new List<RaceEntry>
        {
            new RaceEntry { Grid = 1 },
            new RaceEntry { Grid = 4 },
            new RaceEntry { Grid = null }
        };

        var total = PointCalculationHelper.GetDriversQualificationPoint(entries);
        Assert.Equal(10 + 7, total);
    }

    [Theory]
    [InlineData(false, null, -25)]
    [InlineData(true, 1, 25)]
    [InlineData(true, 2, 18)]
    [InlineData(true, 3, 15)]
    [InlineData(true, 4, 12)]
    [InlineData(true, 5, 10)]
    [InlineData(true, 6, 8)]
    [InlineData(true, 7, 6)]
    [InlineData(true, 8, 4)]
    [InlineData(true, 9, 2)]
    [InlineData(true, 10, 1)]
    [InlineData(true, 11, 0)]
    [InlineData(true, null, 0)]
    public void GetDriverRacePoint_ReturnsExpected(bool isFinished, int? pos, int expected)
    {
        var actual = PointCalculationHelper.GetDriverRacePoint(pos, isFinished);
        Assert.Equal(expected, actual);
    }
    

    [Theory]
    // neither top10 -> -1
    [InlineData(11, 12, -1)]
    // one top10 -> 1
    [InlineData(5, 11, 1)]
    // both top10 -> 3
    [InlineData(5, 4, 1)]
    // one top3 -> 5
    [InlineData(2, 5, 5)]
    // both top3 -> 10
    [InlineData(1, 2, 10)]
    public void GetConstructorQualificationPoint_VariousScenarios(int d1, int d2, int expected)
    {
        var actual = PointCalculationHelper.GetConstructorQualificationPoint(d1, d2);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GetDriverPositionGainPoint_ReturnsGainOrZeroIfNull()
    {
        Assert.Equal(2, PointCalculationHelper.GetDriverPositionGainPoint(5, 3));
        Assert.Equal(0, PointCalculationHelper.GetDriverPositionGainPoint(null, 3));
        Assert.Equal(0, PointCalculationHelper.GetDriverPositionGainPoint(5, null));
        Assert.Equal(0, PointCalculationHelper.GetDriverPositionGainPoint(null, null));
    }

    [Theory]
    [InlineData(1, 3)]
    [InlineData(null, 0)]
    [InlineData(2, 0)]
    public void GetDriverFastestLapPoint_OnlyFirstGetsPoints(int? fastestLap, int expected)
    {
        var actual = PointCalculationHelper.GetDriverFastestLapPoint(fastestLap);
        Assert.Equal(expected, actual);
    }
}