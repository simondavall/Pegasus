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
            var user = await _userManager.FindByEmailAsync(username);
            if (user == null || ! await _userManager.CheckPasswordAsync(user, password))
            {
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
