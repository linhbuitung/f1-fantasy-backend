using System;
using System.Collections.Generic;

namespace F1FantasyWorker.Core.Common;

public partial class Constructor
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;

    public string? ImgUrl { get; set; }

    public string CountryId { get; set; } = null!;

    public virtual ICollection<AspNetUser> AspNetUsers { get; set; } = new List<AspNetUser>();

    public virtual Country Country { get; set; } = null!;

    public virtual ICollection<DriverPrediction> DriverPredictions { get; set; } = new List<DriverPrediction>();
}
