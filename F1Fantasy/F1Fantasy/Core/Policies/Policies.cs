using Microsoft.AspNetCore.Authorization;

namespace F1Fantasy.Core.Policies;

public static class Policies
{
    public const string CanOperateOnOwnResource = "CanOperateOnOwnResource";

    public static void AddCustomPolicies(AuthorizationOptions options)
    {
        options.AddPolicy(CanOperateOnOwnResource, policy =>
            policy.RequireAssertion(context =>
            {
                if (context.Resource is int userId)
                {
                    var claim = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                    return claim != null && Int32.Parse(claim.Value) == userId;
                }
                return false;
            })
        );
    }
}