using System;
using System.Collections.Generic;

namespace F1FantasyWorker.Core.Common;

public partial class UserLeague
{
    public int LeagueId { get; set; }

    public int UserId { get; set; }

    public bool IsAccepted { get; set; }

    public virtual League League { get; set; } = null!;

    public virtual AspNetUser User { get; set; } = null!;
}
