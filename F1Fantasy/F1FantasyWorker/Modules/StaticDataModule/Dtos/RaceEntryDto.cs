namespace F1FantasyWorker.Modules.StaticDataModule.Dtos;

public class RaceEntryDto
{
    public int? Id { get; set; }

    public int? Position { get; set; }

    public int? Grid { get; set; }

    public int? FastestLap { get; set; }

    public int PointsGained { get; set; }

    public int? DriverId { get; set; }

    public int? RaceId { get; set; }
    
    public int? ConstructorId { get; set; }

    public string? DriverCode { get; set; }

    public string? CircuitCode { get; set; }
    
    public string? ConstructorCode { get; set; }
    public DateOnly? RaceDate { get; set; }

    public bool Finished { get; set; }
    public RaceEntryDto(int? id, 
        int? position, 
        int? grid, 
        int? fastestLap, 
        int? pointsGained, 
        int? driverId,
        string? driverCode, 
        int? raceId, 
        DateOnly? raceDate, 
        int? constructorId, 
        string? constructorCode, 
        string? circuitCode,
        bool finished)
    {
        Id = id ?? null;
        Position = position ?? null;
        Grid = grid ?? null;
        FastestLap = fastestLap ?? null;
        PointsGained = pointsGained ?? 0;
        DriverId = driverId ?? null;
        DriverCode = driverCode;
        ConstructorId = constructorId ?? null;
        ConstructorCode = constructorCode;
        RaceId = raceId ?? null;
        CircuitCode = circuitCode ?? null;
        RaceDate = raceDate;
        Finished = finished;
    }
    
    public RaceEntryDto(int? id, 
        int? position, 
        int? grid, 
        int? fastestLap, 
        int? pointsGained, 
        int? driverId,
        int? raceId, 
        int? constructorId, 
        bool finished)
    {
        Id = id ?? null;
        Position = position ?? null;
        Grid = grid ?? null;
        FastestLap = fastestLap ?? null;
        PointsGained = pointsGained ?? 0;
        DriverId = driverId ?? null;
        ConstructorId = constructorId ?? null;
        RaceId = raceId ?? null;
        Finished = finished;
    }
}