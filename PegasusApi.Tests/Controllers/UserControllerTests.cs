using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using PegasusApi.Data;
using PegasusApi.Models;

namespace PegasusApi.Tests.Controllers
{
    public class UserControllerTests
    {
        private ApplicationDbContext _fakeDbContext;
        private Mock<UserManager<IdentityUser>> _mockUserManager;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase("MockDb");
            _fakeDbContext = new ApplicationDbContext(optionsBuilder.Options);
            
            // This takes a long time to build in test terms (>1 sec)
            _fakeDbContext.Database.EnsureCreated();
            _fakeDbContext.Roles.Add(new IdentityRole("admin"));
            _fakeDbContext.Roles.Add(new IdentityRole("manager"));
            _fakeDbContext.Users.Add(new IdentityUser("simon"));
            _fakeDbContext.SaveChanges();
            var userId = _fakeDbContext.Users.First(x => x.UserName == "simon").Id;
            var roleId = _fakeDbContext.Roles.First(x => x.Name == "admin").Id;
            _fakeDbContext.UserRoles.Add(new IdentityUserRole<string> { UserId = userId, RoleId = roleId });
            _fakeDbContext.SaveChanges();
        }
        
        [SetUp]
        public void EachTestSetup()
        {
            var store = new Mock<IUserStore<IdentityUser>>();
            _mockUserManager = new Mock<UserManager<IdentityUser>>(store.Object, null, null, null, null, null, null, null, null);
        }

        private PegasusApi.Controllers.UserController CreateUserController()
        {
            return new PegasusApi.Controllers.UserController(_fakeDbContext, _mockUserManager.Object);
        }

        //TODO These UserController tests demonstrate a lack of parameter validation. Need to add null checks etc.
        [Test]
        public async Task GetAddRole_AddsRoleToUserManager()
        {
            _mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new IdentityUser());
            _mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()));

            var sut = CreateUserController();
            await sut.AddRole(new UserRolePairModel {UserId = "user-id", RoleName = "role-name"});

            _mockUserManager.Verify(x => x.FindByIdAsync(It.IsAny<string>()), Times.Once);
            _mockUserManager.Verify(x => x.AddToRoleAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Once);
        }
        
        [Test]
        public async Task GetRemoveRole_RemovesRoleFromUserManager()
        {
            _mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new IdentityUser());
            _mockUserManager.Setup(x => x.RemoveFromRoleAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()));

            var sut = CreateUserController();
            await sut.RemoveRole(new UserRolePairModel {UserId = "user-id", RoleName = "role-name"});

            _mockUserManager.Verify(x => x.FindByIdAsync(It.IsAny<string>()), Times.Once);
            _mockUserManager.Verify(x => x.RemoveFromRoleAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Once);
        }
        
        [Test]
        public void GetAllRoles_ReturnsDictionaryOfRoles()
        {
            var sut = CreateUserController();
            var result = sut.GetAllRoles();

            result.Should().BeOfType<Dictionary<string, string>>();
        }
        
        [Test]
        public void GetAllUsers_ReturnsDictionaryOfRoles()
        {
            var sut = CreateUserController();
            var result = sut.GetAllUsers();

            result.Should().BeOfType<List<ApplicationUserModel>>();
        }

    }
}