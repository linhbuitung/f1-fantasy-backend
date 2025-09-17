using F1Fantasy.Core.Policies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace F1Fantasy.Modules.TestModule.Controllers;

[ApiController]
[Route("api/test")]
public class TestController(IAuthorizationService authorizationService) : ControllerBase
{
    [HttpGet("health")]
    public IActionResult GeatHealthCheck()
    {
        return Ok("App is alive and running!");
    }
    
    //check role based access
    [HttpGet("role-check")]
    [Authorize(Roles = "Admin")]
    public IActionResult GetRoleCheckAdmin()
    {
        return Ok("Role check passed! You are an Admin.");
    }
    
    [HttpGet("role-check-super-admin")]
    [Authorize(Roles = "Admin")]
    public IActionResult GetRoleCheckSuperAdmin()
    {
        return Ok("Role check passed! You are a Super Admin.");
    }
    
    [Authorize]
    [HttpGet("myclaims")]
    public IActionResult GetMyClaims()
    {
        var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
        return Ok(claims);
    }
    /*
     Current claim example:
     {
        "type": "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
        "value": "2"
    },
    {
        "type": "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name",
        "value": "name"
    },
    {
        "type": "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress",
        "value": "email"
    },
    {
        "type": "AspNet.Identity.SecurityStamp",
        "value": "securityStampValue"
    },
    {
        "type": "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
        "value": "role string"
    },
    {
        "type": "amr",
        "value": "pwd"
    }*/
    [Authorize]
    [HttpGet("get-own-resource/{userId}")]
    public async Task<IActionResult> GetOwnResource(int userId)
    {
        var authResult = await authorizationService.AuthorizeAsync(User, userId, Policies.CanOperateOnOwnResource);
        if (!authResult.Succeeded)
        {
            return Forbid();
        }
        return Ok();
    }
}