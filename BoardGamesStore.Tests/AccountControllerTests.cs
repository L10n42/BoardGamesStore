using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BoardGamesStore.Controllers;
using BoardGamesStore.Models;
using System.Threading.Tasks;
using BoardGamesStore.ViewModels;
using Microsoft.AspNetCore.Http;

namespace BoardGamesStore.Tests
{
    public class AccountControllerTests
    {
        [Fact]
        public async Task Register_ValidModel_ReturnsRedirectToLogin()
        {
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(),
                null, null, null, null, null, null, null, null
            );

            userManagerMock
                .Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            userManagerMock
                .Setup(um => um.AddToRoleAsync(It.IsAny<ApplicationUser>(), "User"))
                .ReturnsAsync(IdentityResult.Success);

            var signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
                userManagerMock.Object,
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(),
                null, null, null, null
            );

            var controller = new AccountController(userManagerMock.Object, signInManagerMock.Object);
            var model = new RegisterViewModel
            {
                Email = "test@example.com",
                Name = "Test User",
                Password = "StrongPassword123!",
                ConfirmPassword = "StrongPassword123!"
            };

            var result = await controller.Register(model) as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.Equal("Login", result.ActionName);
        }

        [Fact]
        public async Task Register_InvalidModel_ReturnsViewWithModel()
        {
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(),
                null, null, null, null, null, null, null, null
            );

            var signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
                userManagerMock.Object,
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(),
                null, null, null, null
            );

            var controller = new AccountController(userManagerMock.Object, signInManagerMock.Object);
            controller.ModelState.AddModelError("Email", "Email is required");

            var model = new RegisterViewModel
            {
                Email = "",
                Name = "Test User",
                Password = "WeakPassword",
                ConfirmPassword = "WeakPassword"
            };

            var result = await controller.Register(model) as ViewResult;

            Assert.NotNull(result);
            Assert.Equal(model, result.Model);
        }

        [Fact]
        public async Task Register_UserCreationFails_ReturnsViewWithErrors()
        {
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(),
                null, null, null, null, null, null, null, null
            );

            userManagerMock
                .Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Password is too weak" }));

            var signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
                userManagerMock.Object,
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(),
                null, null, null, null
            );

            var controller = new AccountController(userManagerMock.Object, signInManagerMock.Object);
            var model = new RegisterViewModel
            {
                Email = "test@example.com",
                Name = "Test User",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            };

            var result = await controller.Register(model) as ViewResult;

            Assert.NotNull(result);
            Assert.True(controller.ModelState.ErrorCount > 0);
            Assert.Contains(controller.ModelState.Values, v => v.Errors.Any(e => e.ErrorMessage == "Password is too weak"));
        }

    }
}
