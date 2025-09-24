using System;
using System.Collections.Generic;

namespace F1FantasyWorker.Core.Common;

public partial class AspNetUser
{
    public int Id { get; set; }

    public string? DisplayName { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public bool AcceptNotification { get; set; }

    public int ConsecutiveActiveDays { get; set; }

    public DateTime? LastActiveAt { get; set; }

    public int AskAiCredits { get; set; }

    public int? ConstructorId { get; set; }

    public int? DriverId { get; set; }

    public string? CountryId { get; set; }

    public string? UserName { get; set; }

    public string? NormalizedUserName { get; set; }

    public string? Email { get; set; }

    public string? NormalizedEmail { get; set; }

    public bool EmailConfirmed { get; set; }

    public string? PasswordHash { get; set; }

    public string? SecurityStamp { get; set; }

    public string? ConcurrencyStamp { get; set; }

    public string? PhoneNumber { get; set; }

    public bool PhoneNumberConfirmed { get; set; }

    public bool TwoFactorEnabled { get; set; }

    public DateTime? LockoutEnd { get; set; }

    public bool LockoutEnabled { get; set; }

    public int AccessFailedCount { get; set; }

    public virtual ICollection<AspNetUserClaim> AspNetUserClaims { get; set; } = new List<AspNetUserClaim>();

    public virtual ICollection<AspNetUserLogin> AspNetUserLogins { get; set; } = new List<AspNetUserLogin>();

    public virtual ICollection<AspNetUserToken> AspNetUserTokens { get; set; } = new List<AspNetUserToken>();

    public virtual Constructor? Constructor { get; set; }

    public virtual Country? Country { get; set; }

    public virtual Driver? Driver { get; set; }

    public virtual ICollection<FantasyLineup> FantasyLineups { get; set; } = new List<FantasyLineup>();

    public virtual ICollection<League> Leagues { get; set; } = new List<League>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Prediction> Predictions { get; set; } = new List<Prediction>();

    public virtual ICollection<UserLeague> UserLeagues { get; set; } = new List<UserLeague>();

    public virtual ICollection<AspNetRole> Roles { get; set; } = new List<AspNetRole>();
}
