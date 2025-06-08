using System;
using System.Collections.Generic;

namespace F1FantasyWorker.Core.Common;

public partial class Notification
{
    public int Id { get; set; }

    public string Header { get; set; } = null!;

    public string Content { get; set; } = null!;

    public int CreatedAt { get; set; }

    public int UserId { get; set; }

    public virtual User User { get; set; } = null!;
}
