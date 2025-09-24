using System;
using System.Collections.Generic;

namespace F1FantasyWorker.Core.Common;

public partial class FantasyLineup
{
    public int Id { get; set; }

    public int TotalAmount { get; set; }

    public int TransfersMade { get; set; }

    public int PointsGained { get; set; }

    public int UserId { get; set; }

    public int RaceId { get; set; }

    public virtual ICollection<PowerupFantasyLineup> PowerupFantasyLineups { get; set; } = new List<PowerupFantasyLineup>();

    public virtual Race Race { get; set; } = null!;

    public virtual AspNetUser User { get; set; } = null!;

    public virtual ICollection<Constructor> Constructors { get; set; } = new List<Constructor>();

    public virtual ICollection<Driver> Drivers { get; set; } = new List<Driver>();
}
