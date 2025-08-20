using System;
using System.Collections.Generic;

namespace F1FantasyWorker.Core.Common;

public partial class Powerup
{
    public int Id { get; set; }

    public string Type { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string ImgUrl { get; set; } = null!;

    public virtual ICollection<PowerupFantasyLineup> PowerupFantasyLineupPowerupId1Navigations { get; set; } = new List<PowerupFantasyLineup>();

    public virtual ICollection<PowerupFantasyLineup> PowerupFantasyLineupPowerups { get; set; } = new List<PowerupFantasyLineup>();
}
