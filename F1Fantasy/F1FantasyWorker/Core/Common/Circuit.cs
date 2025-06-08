using System;
using System.Collections.Generic;

namespace F1FantasyWorker.Core.Common;

public partial class Circuit
{
    public int Id { get; set; }

    public string CircuitName { get; set; } = null!;

    public decimal Latitude { get; set; }

    public decimal Longtitude { get; set; }

    public string? ImgUrl { get; set; }

    public string Code { get; set; } = null!;

    public string Country { get; set; } = null!;

    public string Locality { get; set; } = null!;

    public virtual ICollection<Prediction> Predictions { get; set; } = new List<Prediction>();

    public virtual ICollection<Race> Races { get; set; } = new List<Race>();
}
