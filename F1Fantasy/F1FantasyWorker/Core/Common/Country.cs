using System;
using System.Collections.Generic;

namespace F1FantasyWorker.Core.Common;

public partial class Country
{
    public string Id { get; set; } = null!;

    public string ShortName { get; set; } = null!;

    public List<string> Nationalities { get; set; } = null!;

    public virtual ICollection<AspNetUser> AspNetUsers { get; set; } = new List<AspNetUser>();

    public virtual ICollection<Circuit> Circuits { get; set; } = new List<Circuit>();

    public virtual ICollection<Constructor> Constructors { get; set; } = new List<Constructor>();

    public virtual ICollection<Driver> Drivers { get; set; } = new List<Driver>();
}
