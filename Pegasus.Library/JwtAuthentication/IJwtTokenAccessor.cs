using Pegasus.Library.JwtAuthentication.Models;

namespace Pegasus.Library.JwtAuthentication
{
    public interface IJwtTokenAccessor
    {
        TokenWithClaimsPrincipal GetAccessTokenWithClaimsPrincipal(AuthenticatedUser user);
    }
}