using System;
using System.Collections.Generic;

namespace F1FantasyWorker.Core.Common;

public partial class PowerupFantasyLineup
{
    public int FantasyLineupId { get; set; }

    public int PowerupId { get; set; }

    public int FantasyLineupId1 { get; set; }

    public int PowerupId1 { get; set; }

    public int? DriverId { get; set; }

    public virtual Driver? Driver { get; set; }

    public virtual FantasyLineup FantasyLineup { get; set; } = null!;

    public virtual FantasyLineup FantasyLineupId1Navigation { get; set; } = null!;

    public virtual Powerup Powerup { get; set; } = null!;

    public virtual Powerup PowerupId1Navigation { get; set; } = null!;
}
