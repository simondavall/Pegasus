using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PegasusApi.Data;
using PegasusApi.Library.JwtAuthentication;
using PegasusApi.Models;


namespace PegasusApi.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TokenController : Controller
    {
        private readonly IApplicationDbContext _context;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly ILogger<TokenController> _logger;
        private readonly UserManager<IdentityUser> _userManager;


        public TokenController(IApplicationDbContext context, UserManager<IdentityUser> userManager, 
            IJwtTokenGenerator jwtTokenGenerator, ILogger<TokenController> logger)
        {
            _context = context;
            _jwtTokenGenerator = jwtTokenGenerator;
            _logger = logger;
            _userManager = userManager;
        }

        [AllowAnonymous]
        [Route("/token")]
        [HttpPost]
        public async Task<IActionResult> CreateToken(string username, string password, string grantType)
        {
            var user = await _userManager.FindByEmailAsync(username);
            if (user == null)
            {
                _logger.LogWarning("Failed to find user {Username}", username);
                return NotFound();
            }
            if (! await _userManager.CheckPasswordAsync(user, password))
            {
                _logger.LogWarning("Failed login attempt by {Username}", username);
                return BadRequest();
            }

            return new ObjectResult(GenerateTokenModel(user));
        }

        [AllowAnonymous]
        [Route("/refresh_token")]
        [HttpPost]
        public async Task<IActionResult> RefreshToken(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("Failed to find user {Userid}", userId);
                return NotFound();
            }

            return new ObjectResult(GenerateTokenModel(user));
        }

        [AllowAnonymous]
        [Route("/2fa_token")]
        [HttpPost]
        public async Task<IActionResult> Create2FaToken(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("Failed to find user {Userid}", userId);
                return NotFound();
            }

            var claims = new List<Claim> {new Claim("amr", "mfa")};
            return new ObjectResult(GenerateTokenModel(user, claims));
        }

        private dynamic GenerateTokenModel(IdentityUser user, ICollection<Claim> claims = null)
        {
            if (user == null)
            {
                _logger.LogWarning("Null user passed to " +  nameof(GenerateTokenModel));
                return new TokenModel();
            }
            
            var roles = _context.GetRolesForUser(user);
            
            claims ??= new List<Claim>();
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            string accessToken;

            try
            {
                accessToken = _jwtTokenGenerator.GenerateAccessToken(user, claims);
            }
            catch(Exception ex)
            {
                _logger.LogCritical(ex, "Failed to generate access token for user {Username} with userid {Userid}. TokenOptions not setup correctly",
                    user.UserName, user.Id);
                ModelState.AddModelError("TokenError", $"Unable to login to the system at the moment. Please try later.");
                return new TokenModel();
            }

            var tokenModel = new TokenModel
            {
                AccessToken = accessToken,
                Username = user.UserName,
                UserId = user.Id,
                RequiresTwoFactor = user.TwoFactorEnabled
            };

            return tokenModel;
        }
    }
}
