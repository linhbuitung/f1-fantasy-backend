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
    public async Task<IActionResult> MakeNewMainRacePrediction(int userId, MainRacePredictionCreateAsNewDto mainRacePrediction)
    {
        var authResult = await authorizationService.AuthorizeAsync(User, userId, AuthPolicies.CanOperateOnOwnResource);
        if (!authResult.Succeeded)
        {
            return Forbid();
        }
        
        var predictionId = await askAiService.MakeMainRacePredictionAsNewPredictionAsync(userId, mainRacePrediction);
        var prediction = await askAiService.GetPredictionDetailByIdAsync(predictionId);
        return Ok(prediction);
    }
    
    [Authorize]
    [HttpPost("user/{userId:int}/prediction/qualifying")]
    public async Task<IActionResult> MakeNewQualifyingPrediction(int userId, QualifyingPredictionCreateDto qualifyingPredictionCreateDto)
    {
        var authResult = await authorizationService.AuthorizeAsync(User, userId, AuthPolicies.CanOperateOnOwnResource);
        if (!authResult.Succeeded)
        {
            return Forbid();
        }
        
        var predictionId = await askAiService.MakeQualifyingPredictionAsync(userId, qualifyingPredictionCreateDto);
        var prediction = await askAiService.GetPredictionDetailByIdAsync(predictionId);

        return Ok(prediction);
    }
    
    
    [Authorize]
    [HttpPost("user/{userId:int}/prediction/{predictionId:int}/main-race")]
    public async Task<IActionResult> MakeMainRacePredictionFromAlreadyMadeQualifyingPrediction(int userId, int predictionId, MainRacePredictionCreateAsAdditionDto mainRacePredictionCreateAsAdditionDto)
    {
        var authResult = await authorizationService.AuthorizeAsync(User, userId, AuthPolicies.CanOperateOnOwnResource);
        if (!authResult.Succeeded)
        {
            return Forbid();
        }
        
        var updatedPredictionId = await askAiService.MakeMainRacePredictionFromAlreadyMadeQualifyingPredictionAsync(userId, predictionId, mainRacePredictionCreateAsAdditionDto);
        var prediction = await askAiService.GetPredictionDetailByIdAsync(updatedPredictionId);

        return Ok(prediction);
    }
    
    [Authorize]
    [HttpGet("drivers/main-race")]
    public async Task<IActionResult> GetMlPickableDriversForMainRace()
    {
        var drivers = await askAiService.GetMlPickableDriversAsync(Extensions.AskAiClient.PredictionType.MainRace);
        return Ok(drivers);
    }

    [Authorize]
    [HttpGet("drivers/qualifying")]
    public async Task<IActionResult> GetMlPickableDriversForQualifying()
    {
        var drivers = await askAiService.GetMlPickableDriversAsync(Extensions.AskAiClient.PredictionType.Qualifying);
        return Ok(drivers);
    }

    [Authorize]
    [HttpGet("constructors/main-race")]
    public async Task<IActionResult> GetMlPickableConstructorsForMainRace()
    {
        var constructors =
            await askAiService.GetMlPickableConstructorsAsync(Extensions.AskAiClient.PredictionType.MainRace);
        return Ok(constructors);
    }

    [Authorize]
    [HttpGet("constructors/qualifying")]
    public async Task<IActionResult> GetMlPickableConstructorsForQualifying()
    {
        var constructors =
            await askAiService.GetMlPickableConstructorsAsync(Extensions.AskAiClient.PredictionType.Qualifying);
        return Ok(constructors);
    }

    [Authorize]
    [HttpGet("circuits/main-race")]
    public async Task<IActionResult> GetMlPickableCircuitsForMainRace()
    {
        var circuits = await askAiService.GetMlPickableCircuitsAsync(Extensions.AskAiClient.PredictionType.MainRace);
        return Ok(circuits);
    }

    [Authorize]
    [HttpGet("circuits/qualifying")]
    public async Task<IActionResult> GetMlPickableCircuitsForQualifying()
    {
        var circuits = await askAiService.GetMlPickableCircuitsAsync(Extensions.AskAiClient.PredictionType.Qualifying);
        return Ok(circuits);
    }
}