using System;
using System.Collections.Generic;

namespace F1FantasyWorker.Core.Common;

public partial class FantasyLineupDriver
{
    public int FantasyLineupId { get; set; }

    public int DriverId { get; set; }

    public virtual Driver Driver { get; set; } = null!;

    public virtual FantasyLineup FantasyLineup { get; set; } = null!;
}
