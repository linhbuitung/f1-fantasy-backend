using System;
using System.Collections.Generic;

namespace F1FantasyWorker.Core.Common;

public partial class Season
{
    public int Id { get; set; }

    public int Year { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Race> Races { get; set; } = new List<Race>();
}
