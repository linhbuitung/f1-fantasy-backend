using System;
using System.Collections.Generic;

namespace F1FantasyWorker.Core.Common;

public partial class DriverPrediction
{
    public int Id { get; set; }

    public int? GridPosition { get; set; }

    public int? FinalPosition { get; set; }

    public bool Crashed { get; set; }

    public int PredictionId { get; set; }

    public int DriverId { get; set; }

    public int ConstructorId { get; set; }

    public virtual Constructor Constructor { get; set; } = null!;

    public virtual Driver Driver { get; set; } = null!;

    public virtual Prediction Prediction { get; set; } = null!;
}
