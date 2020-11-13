using Pegasus.Library.JwtAuthentication.Models;
using Pegasus.Library.Models;

namespace Pegasus.Library.JwtAuthentication
{
    public interface IJwtTokenAccessor
    {
        TokenWithClaimsPrincipal GetAccessTokenWithClaimsPrincipal(AuthenticatedUser user);
    }
}