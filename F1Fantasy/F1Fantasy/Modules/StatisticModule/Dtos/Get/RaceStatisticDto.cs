using System.ComponentModel.DataAnnotations;

namespace F1Fantasy.Modules.StatisticModule.Dtos.Get;

public class RaceStatisticDto
{
    public int Id { get; set; }
    
    public required string RaceName { get; set; }

    public int Round { get; set;} 

    public DateOnly RaceDate { get; set; }

    public DateOnly DeadlineDate { get; set; }

    public bool Calculated { get; set; } 
    
    public required CircuitInRaceStatisticDto Circuit { get; set; }
    
    public List<DriverInRaceStatisticDto> DriversStatistics { get; set; }
    
    public List<ConstructorInRaceStatisticDto> ConstructorsStatistics { get; set; }
}

public class CircuitInRaceStatisticDto
{
    public required string CircuitName { get; set; }

    public required string Code { get; set; }

    public required string Country { get; set; }

    public string? ImgUrl { get; set; }
}

public class DriverInRaceStatisticDto
{
    public int Id { get; set; }

    public required string GivenName { get; set; }

    public required string FamilyName { get; set; }
    
    public required string ConstructorName { get; set; }

    public string? ImgUrl { get; set; }
    
    public int? Position { get; set; }
        
    public int? Grid { get; set; }
        
    public int? FastestLap { get; set; }

    [Required]
    public int PointsGained { get; set; }
        
    public bool Finished { get; set; }
}

public class ConstructorInRaceStatisticDto
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public string? ImgUrl { get; set; }
    
    [Required]
    public int PointsGained { get; set; }
}