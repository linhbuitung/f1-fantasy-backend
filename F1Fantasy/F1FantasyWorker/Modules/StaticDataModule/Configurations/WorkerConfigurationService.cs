namespace F1FantasyWorker.Modules.StaticDataModule.Configs;

public class WorkerConfigurationService
{
    public int DelayBetweenRequests { get; }
    public int SyncRequestLimit { get;  }

    public WorkerConfigurationService()
    {
        var delayValue = Environment.GetEnvironmentVariable("DELAY_BETWEEN_REQUESTS");
        DelayBetweenRequests = int.TryParse(delayValue, out var delay) ? delay : 1000;
        
        var syncRequestLimitValue = Environment.GetEnvironmentVariable("SYNC_REQUEST_LIMIT");
        SyncRequestLimit = int.TryParse(syncRequestLimitValue, out var limit) ? limit : 100;
    }
}