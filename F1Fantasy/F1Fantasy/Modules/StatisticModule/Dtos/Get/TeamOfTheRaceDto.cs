namespace F1Fantasy.Modules.StatisticModule.Dtos.Get;

public class TeamOfTheRaceDto
{
    public int Id { get; set; }
    public string RaceName { get; set; }
    
    public int Round { get; set; }
    
    public List<DriverInTeamOfTheRaceDto> Drivers { get; set; }
    
    public List<ConstructorInTeamOfTheRaceDto> Constructors { get; set; }
}
