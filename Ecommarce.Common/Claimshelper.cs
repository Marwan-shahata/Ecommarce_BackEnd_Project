using System.Security.Claims;
using ECommerce.Common;

namespace ECommerce.Common;

public static class ClaimsHelper
{
    public static string GetUserId(ClaimsPrincipal user)
    {
        var userId = user.FindFirstValue(ClaimTypes.UserId)
                     ?? user.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
            throw new InvalidOperationException(
                "UserId claim not found in token. Ensure [Authorize] is applied.");

        return userId;
    }

    public static string? GetEmail(ClaimsPrincipal user)
        => user.FindFirstValue(ClaimTypes.Email)
           ?? user.FindFirstValue(System.Security.Claims.ClaimTypes.Email);

    public static IEnumerable<string> GetRoles(ClaimsPrincipal user)
        => user.Claims
               .Where(c => c.Type == System.Security.Claims.ClaimTypes.Role)
               .Select(c => c.Value);

    public static bool IsAdmin(ClaimsPrincipal user)
        => user.IsInRole(Roles.Admin);
}