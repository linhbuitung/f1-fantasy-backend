using F1Fantasy.Core.Policies;
using F1Fantasy.Modules.AskAiModule.Dtos.Create;
using F1Fantasy.Modules.AskAiModule.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace F1Fantasy.Modules.AskAiModule.Controllers;

[ApiController]
[Route("ask-ai")]
public class AskAiController(
    IAskAIService askAiService, 
    IAuthorizationService authorizationService) : ControllerBase
{
    [Authorize]
    [HttpGet("user/{userId:int}/predictions")]
    public async Task<IActionResult> GetAllPredictionsByUserId(int userId, [FromQuery] int pageNum = 1,
    [FromQuery] int pageSize = 10)
    {
        var authResult = await authorizationService.AuthorizeAsync(User, userId, AuthPolicies.CanOperateOnOwnResource);
        if (!authResult.Succeeded)
        {
            return Forbid();
        }
        
        var prediction = await askAiService.GetPagedPredictionsByUserIdAsync(userId, pageNum, pageSize);
        return Ok(prediction);
    }
    
    [Authorize]
    [HttpGet("user/{userId:int}/prediction/{predictionId:int}")]
    public async Task<IActionResult> GetPredictionDetailByUserId(int userId, int predictionId)
    {
        var authResult = await authorizationService.AuthorizeAsync(User, userId, AuthPolicies.CanOperateOnOwnResource);
        if (!authResult.Succeeded)
        {
            return Forbid();
        }
        
        var prediction = await askAiService.GetPredictionDetailByIdAsync(predictionId);
        if (prediction.UserId != userId)
        {
            return Forbid();
        }
        return Ok(prediction);
    }
    
    [Authorize]
    [HttpPost("user/{userId:int}/prediction/main-race")]
    public async Task<IActionResult> MakeQualifyingPrediction(int userId, MainRacePredictionCreateDto mainRacePrediction)
    {
        var authResult = await authorizationService.AuthorizeAsync(User, userId, AuthPolicies.CanOperateOnOwnResource);
        if (!authResult.Succeeded)
        {
            return Forbid();
        }
        
        var prediction = await askAiService.MakeMainRacePredictionAsNewPredictionAsync(userId, mainRacePrediction);
        if (prediction.UserId != userId)
        {
            return Forbid();
        }
        return Ok(prediction);
    }
}