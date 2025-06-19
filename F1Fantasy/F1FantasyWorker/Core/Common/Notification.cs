using System;
using System.Collections.Generic;

namespace F1FantasyWorker.Core.Common;

public partial class Notification
{
    public Guid Id { get; set; }

    public string Header { get; set; } = null!;

    public string Content { get; set; } = null!;

    public int CreatedAt { get; set; }

    public Guid UserId { get; set; }

    public virtual AspNetUser User { get; set; } = null!;
}
