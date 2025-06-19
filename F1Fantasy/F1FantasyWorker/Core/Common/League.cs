using System;
using System.Collections.Generic;

namespace F1FantasyWorker.Core.Common;

public partial class League
{
    public Guid Id { get; set; }

    public int MaxPlayersNum { get; set; }

    public string Type { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public Guid UserId { get; set; }

    public virtual AspNetUser User { get; set; } = null!;

    public virtual ICollection<AspNetUser> Users { get; set; } = new List<AspNetUser>();
}
