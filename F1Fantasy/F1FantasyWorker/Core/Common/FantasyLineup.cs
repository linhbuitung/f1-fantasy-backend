using System;
using System.Collections.Generic;

namespace F1FantasyWorker.Core.Common;

public partial class FantasyLineup
{
    public int Id { get; set; }

    public int TotalAmount { get; set; }

    public int TransferPointsDeducted { get; set; }

    public int PointsGained { get; set; }

    public int UserId { get; set; }

    public int RaceId { get; set; }

    public virtual ICollection<FantasyLineupDriver> FantasyLineupDriverFantasyLineupId1Navigations { get; set; } = new List<FantasyLineupDriver>();

    public virtual ICollection<FantasyLineupDriver> FantasyLineupDriverFantasyLineups { get; set; } = new List<FantasyLineupDriver>();

    public virtual ICollection<PowerupFantasyLineup> PowerupFantasyLineupFantasyLineupId1Navigations { get; set; } = new List<PowerupFantasyLineup>();

    public virtual ICollection<PowerupFantasyLineup> PowerupFantasyLineupFantasyLineups { get; set; } = new List<PowerupFantasyLineup>();

    public virtual Race Race { get; set; } = null!;

    public virtual AspNetUser User { get; set; } = null!;
}
