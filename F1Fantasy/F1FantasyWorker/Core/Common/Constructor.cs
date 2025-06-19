using System;
using System.Collections.Generic;

namespace F1FantasyWorker.Core.Common;

public partial class Constructor
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string Nationality { get; set; } = null!;

    public string Code { get; set; } = null!;

    public string? ImgUrl { get; set; }

    public virtual ICollection<AspNetUser> AspNetUsers { get; set; } = new List<AspNetUser>();

    public virtual ICollection<DriverPrediction> DriverPredictions { get; set; } = new List<DriverPrediction>();
}
