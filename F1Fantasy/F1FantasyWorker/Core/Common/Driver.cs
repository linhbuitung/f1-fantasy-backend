using System;
using System.Collections.Generic;

namespace F1FantasyWorker.Core.Common;

public partial class Driver
{
    public int Id { get; set; }

    public string GivenName { get; set; } = null!;

    public string FamilyName { get; set; } = null!;

    public DateOnly DateOfBirth { get; set; }

    public string Code { get; set; } = null!;

    public string? ImgUrl { get; set; }

    public string CountryId { get; set; } = null!;

    public int? PickableItemId { get; set; }

    public int Price { get; set; }

    public virtual ICollection<AspNetUser> AspNetUsers { get; set; } = new List<AspNetUser>();

    public virtual Country Country { get; set; } = null!;

    public virtual ICollection<DriverPrediction> DriverPredictions { get; set; } = new List<DriverPrediction>();

    public virtual PickableItem? PickableItem { get; set; }

    public virtual ICollection<PowerupFantasyLineup> PowerupFantasyLineups { get; set; } = new List<PowerupFantasyLineup>();

    public virtual ICollection<RaceEntry> RaceEntries { get; set; } = new List<RaceEntry>();

    public virtual ICollection<FantasyLineup> FantasyLineups { get; set; } = new List<FantasyLineup>();
}
