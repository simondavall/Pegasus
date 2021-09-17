using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace PegasusApi.Services
{
    /// <summary>
    /// Wrapper created for UserManager class in order to create lighter weight tests that run faster
    /// Add only methods that are actually used and tested to this wrapping class
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    public interface IApplicationUserManager<TUser> where TUser : class
    {
        Task<bool> CheckPasswordAsync(TUser user, string password);
        Task<TUser> FindByEmailAsync(string email);
        Task<TUser> FindByIdAsync(string userId);
    }

    public class ApplicationUserManager<TUser> : UserManager<TUser>, IApplicationUserManager<TUser> where TUser : class
    {

        public ApplicationUserManager(IUserStore<TUser> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<TUser> passwordHasher, IEnumerable<IUserValidator<TUser>> userValidators, IEnumerable<IPasswordValidator<TUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<TUser>> logger) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
        }

        public override async Task<bool> CheckPasswordAsync(TUser user, string password)
        {
            return await base.CheckPasswordAsync(user, password);
        }
        
        public override async Task<TUser> FindByEmailAsync(string email)
        {
            return await base.FindByEmailAsync(email);
        }
        
        public override async Task<TUser> FindByIdAsync(string userId)
        {
            return await base.FindByIdAsync(userId);
        }
    }
}
