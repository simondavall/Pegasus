using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PegasusApi.Data;
using PegasusApi.Library.JwtAuthentication;
using PegasusApi.Models;
using PegasusApi.Services;


namespace PegasusApi.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TokenController : Controller
    {
        private readonly IApplicationDbContext _context;
        private readonly IApplicationUserManager<IdentityUser> _userManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public TokenController(IApplicationDbContext context, IApplicationUserManager<IdentityUser> userManager, IJwtTokenGenerator jwtTokenGenerator)
        {
            _context = context;
            _userManager = userManager;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        [AllowAnonymous]
        [Route("/token")]
        [HttpPost]
        public async Task<IActionResult> CreateToken(string username, string password, string grantType)
        {
            var user = await IsValidUsernameAndPassword(username, password);
            if (user == null)
            {
                //TODO Look into whether I should be returning a message with the bad request response
                return BadRequest();
            }

            return new ObjectResult(GenerateToken(user));
        }

        [AllowAnonymous]
        [Route("/refresh_token")]
        [HttpPost]
        public async Task<IActionResult> RefreshToken(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest();
            }

            return new ObjectResult(GenerateToken(user));
        }

        [AllowAnonymous]
        [Route("/2fa_token")]
        [HttpPost]
        public async Task<IActionResult> Create2FaToken(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest();
            }

            var claims = new List<Claim> {new Claim("amr", "mfa")};
            return new ObjectResult(GenerateToken(user, claims));
        }

        //TODO Change this back to returning a boolean, and change signature to (user, password))
        //do the find user in the calling code so that this just checks for valid password.
        private async Task<IdentityUser> IsValidUsernameAndPassword(string username, string password)
        {
            var user = await _userManager.FindByEmailAsync(username);
            var result = await _userManager.CheckPasswordAsync(user, password);
            return result ? user : null;
        }

        private dynamic GenerateToken(IdentityUser user, ICollection<Claim> claims = null)
        {
            if (user == null)
            {
                return new TokenModel();
            }
            
            var roles = _context.GetRolesForUser(user);
            
            claims ??= new List<Claim>();
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var output = new TokenModel
            {
                AccessToken = _jwtTokenGenerator.GenerateAccessToken(user, claims),
                Username = user.UserName,
                UserId = user.Id,
                RequiresTwoFactor = user.TwoFactorEnabled
            };

            return output;
        }
    }
}
