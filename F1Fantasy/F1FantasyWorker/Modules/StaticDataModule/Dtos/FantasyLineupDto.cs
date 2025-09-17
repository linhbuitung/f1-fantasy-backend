namespace F1FantasyWorker.Modules.StaticDataModule.Dtos;

public class FantasyLineupDto
{
    public int? Id { get; set; }

    public int TotalAmount { get; set; }

    public int TransferPointsDeducted { get; set; }

    public int TransfersMade { get; set; }

    public int UserId { get; set; }

    public int RaceId { get; set; }
    
    public FantasyLineupDto(int userId, int raceId)
    {
        UserId = userId;
        RaceId = raceId;
    }
    public FantasyLineupDto(int? id, int totalAmount, int transferPointsDeducted, int transfersMade, int userId, int raceId)
    {
        Id = id ?? null;
        TotalAmount = totalAmount;
        TransferPointsDeducted = transferPointsDeducted;
        TransfersMade = transfersMade;
        UserId = userId;
        RaceId = raceId;
    }
}