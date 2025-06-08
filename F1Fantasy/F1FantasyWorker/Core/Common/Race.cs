using System;
using System.Collections.Generic;

namespace F1FantasyWorker.Core.Common;

public partial class Race
{
    public int Id { get; set; }

    public DateOnly Date { get; set; }

    public DateOnly DeadlineDate { get; set; }

    public int CircuitId { get; set; }

    public bool Calculated { get; set; }

    public virtual Circuit Circuit { get; set; } = null!;

    public virtual ICollection<FantasyLineup> FantasyLineups { get; set; } = new List<FantasyLineup>();

    public virtual ICollection<RaceEntry> RaceEntries { get; set; } = new List<RaceEntry>();
}
