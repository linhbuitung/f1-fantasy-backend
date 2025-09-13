using System;
using System.Collections.Generic;

namespace F1FantasyWorker.Core.Common;

public partial class RaceEntry
{
    public int Id { get; set; }

    public int? Position { get; set; }

    public int? Grid { get; set; }

    public int? FastestLap { get; set; }

    public int PointsGained { get; set; }

    public int DriverId { get; set; }

    public int RaceId { get; set; }

    public int ConstructorId { get; set; }

    public bool Finished { get; set; }

    public virtual Constructor Constructor { get; set; } = null!;

    public virtual Driver Driver { get; set; } = null!;

    public virtual Race Race { get; set; } = null!;
}
