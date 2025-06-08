using System;
using System.Collections.Generic;

namespace F1FantasyWorker.Core.Common;

public partial class Prediction
{
    public int Id { get; set; }

    public DateOnly DatePredicted { get; set; }

    public int PredictYear { get; set; }

    public bool Rain { get; set; }

    public int UserId { get; set; }

    public int CircuitId { get; set; }

    public virtual Circuit Circuit { get; set; } = null!;

    public virtual ICollection<DriverPrediction> DriverPredictions { get; set; } = new List<DriverPrediction>();

    public virtual User User { get; set; } = null!;
}
