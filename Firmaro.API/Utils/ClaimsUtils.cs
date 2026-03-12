using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Firmaro.API.Utils
{
    public static class ClaimsUtils
    {
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            return Guid.Parse(user.FindFirstValue(JwtRegisteredClaimNames.Sub)
                ?? user.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }
    }
}
