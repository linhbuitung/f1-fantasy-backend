using System;
using System.Collections.Generic;

namespace F1FantasyWorker.Core.Common;

public partial class Driver
{
    public int Id { get; set; }

    public string GivenName { get; set; } = null!;

    public string FamilyName { get; set; } = null!;

    public DateOnly DateOfBirth { get; set; }

    public string Nationality { get; set; } = null!;

    public string Code { get; set; } = null!;

    public string? ImgUrl { get; set; }

    public virtual ICollection<DriverPrediction> DriverPredictions { get; set; } = new List<DriverPrediction>();

    public virtual ICollection<RaceEntry> RaceEntries { get; set; } = new List<RaceEntry>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();

    public virtual ICollection<FantasyLineup> FantasyLineups { get; set; } = new List<FantasyLineup>();
}
