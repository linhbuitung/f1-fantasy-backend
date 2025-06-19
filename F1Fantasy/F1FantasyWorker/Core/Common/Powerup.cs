using System;
using System.Collections.Generic;

namespace F1FantasyWorker.Core.Common;

public partial class Powerup
{
    public Guid Id { get; set; }

    public string Type { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string ImgUrl { get; set; } = null!;

    public virtual ICollection<FantasyLineup> FantasyLineups { get; set; } = new List<FantasyLineup>();
}
