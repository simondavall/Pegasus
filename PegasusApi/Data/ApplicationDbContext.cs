using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace PegasusApi.Data
{
    public interface IApplicationDbContext
    {
        IEnumerable<string> GetRolesForUser(IdentityUser user);
    }

    public class ApplicationDbContext : IdentityDbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public IEnumerable<string> GetRolesForUser(IdentityUser user)
        {
            var roles = from ur in UserRoles
                join r in Roles on ur.RoleId equals r.Id
                where ur.UserId == user.Id
                select r.Name;

            return roles.ToList();
        }
    }
}
