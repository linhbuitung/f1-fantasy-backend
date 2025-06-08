using System;
using System.Collections.Generic;

namespace F1FantasyWorker.Core.Common;

public partial class User
{
    public int Id { get; set; }

    public string DisplayName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateOnly DateOfBirth { get; set; }

    public string Nationality { get; set; } = null!;

    public string Password { get; set; } = null!;

    public DateOnly CreatedAt { get; set; }

    public DateTime LastLogin { get; set; }

    public bool AcceptNotification { get; set; }

    public int LoginStreak { get; set; }

    public int ConstructorId { get; set; }

    public int DriverId { get; set; }

    public int Role { get; set; }

    public string RefreshToken { get; set; } = null!;

    public DateTime? RefreshTokenExp { get; set; }

    public string Salt { get; set; } = null!;

    public virtual Constructor Constructor { get; set; } = null!;

    public virtual Driver Driver { get; set; } = null!;

    public virtual ICollection<FantasyLineup> FantasyLineups { get; set; } = new List<FantasyLineup>();

    public virtual ICollection<League> Leagues { get; set; } = new List<League>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Prediction> Predictions { get; set; } = new List<Prediction>();

    public virtual ICollection<League> LeaguesNavigation { get; set; } = new List<League>();
}
