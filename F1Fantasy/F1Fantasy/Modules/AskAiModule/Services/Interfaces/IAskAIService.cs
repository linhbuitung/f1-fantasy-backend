namespace F1Fantasy.Modules.AskAiModule.Services.Interfaces;

public interface IAskAIService
{
    Task AddAskAiCreditAsync(int userId);
    Task AddAskAiCreditAsync(string userId);
}