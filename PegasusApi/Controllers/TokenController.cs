using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PegasusApi.Data;
using PegasusApi.Library.JwtAuthentication;
using PegasusApi.Models;


namespace PegasusApi.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TokenController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public TokenController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IJwtTokenGenerator jwtTokenGenerator)
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
            if (await IsValidUsernameAndPassword(username, password))
            {
                var user = await _userManager.FindByEmailAsync(username);
                return new ObjectResult(GenerateToken(user));
            }
            else
            {
                return BadRequest();
            }
        }

        [AllowAnonymous]
        [Route("/refresh_token")]
        [HttpPost]
        public async Task<IActionResult> RefreshToken(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return new ObjectResult(GenerateToken(user));
        }

        [AllowAnonymous]
        [Route("/2fa_token")]
        [HttpPost]
        public async Task<IActionResult> Create2FaToken(string userId)
        {
            var claims = new List<Claim> {new Claim("amr", "mfa")};
            var user = await _userManager.FindByIdAsync(userId);
            return new ObjectResult(GenerateToken(user, claims));
        }

        private async Task<bool> IsValidUsernameAndPassword(string username, string password)
        {
            var user = await _userManager.FindByEmailAsync(username);
            var result = await _userManager.CheckPasswordAsync(user, password);
            return result;
        }

        private dynamic GenerateToken(IdentityUser user, ICollection<Claim> claims = null)
        {
            var roles = from ur in _context.UserRoles
                join r in _context.Roles on ur.RoleId equals r.Id
                where ur.UserId == user.Id
                select new {ur.UserId, ur.RoleId, r.Name};

            claims ??= new List<Claim>();
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name));
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
