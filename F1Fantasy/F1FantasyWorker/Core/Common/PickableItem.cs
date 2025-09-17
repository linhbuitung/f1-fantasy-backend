using System;
using System.Collections.Generic;

namespace F1FantasyWorker.Core.Common;

public partial class PickableItem
{
    public int Id { get; set; }

    public virtual ICollection<Constructor> Constructors { get; set; } = new List<Constructor>();

    public virtual ICollection<Driver> Drivers { get; set; } = new List<Driver>();
}
