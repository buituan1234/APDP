using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Controllers; // Thêm không gian tên này
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
using SIMS.Controllers;
using SIMS.Models;
using Microsoft.EntityFrameworkCore;

namespace SIMS.Tests
{
	public class AdminControllerTests
	{
		private readonly Mock<SimsContext> _mockContext;
		private readonly AdminsController _controller;

		public AdminControllerTests()
		{
			_mockContext = new Mock<SimsContext>();
			_controller = new AdminsController(_mockContext.Object);

			// Mock services
			var services = new ServiceCollection();
			services.AddMvc();
			services.AddSingleton<ITempDataDictionaryFactory, TempDataDictionaryFactory>();
			services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
					.AddCookie();

			var serviceProvider = services.BuildServiceProvider();
			var httpContext = new DefaultHttpContext
			{
				RequestServices = serviceProvider
			};

			_controller.ControllerContext = new ControllerContext
			{
				HttpContext = httpContext,
				RouteData = new RouteData(),
				ActionDescriptor = new ControllerActionDescriptor() // Sửa để khởi tạo đúng loại ActionDescriptor
			};
		}

		[Fact]
		public async Task Login_ValidAdmin_RedirectsToDashboard()
		{
			// Arrange
			var admin = new Admin { AdminId = 1, UserName = "admin", Password = "password" };
			var role = new Role { RoleId = 1 };

			var mockSetAdmins = CreateDbSetMock(new List<Admin> { admin });
			var mockSetRoles = CreateDbSetMock(new List<Role> { role });

			_mockContext.Setup(c => c.Admins).Returns(mockSetAdmins.Object);
			_mockContext.Setup(c => c.Roles).Returns(mockSetRoles.Object);

			var model = new Admin { UserName = "admin", Password = "password" };

			// Act
			var result = await _controller.Login(model) as RedirectToActionResult;

			// Assert
			Assert.NotNull(result);
			Assert.Equal("DashBoard", result.ActionName);
			Assert.Equal("Admins", result.ControllerName);
		}

		[Fact]
		public async Task Login_InvalidAdmin_ReturnsViewWithErrorMessage()
		{
			// Arrange
			var model = new Admin { UserName = "wrong", Password = "wrong" };

			var mockSetAdmins = CreateDbSetMock(new List<Admin>());
			var mockSetRoles = CreateDbSetMock(new List<Role>());

			_mockContext.Setup(c => c.Admins).Returns(mockSetAdmins.Object);
			_mockContext.Setup(c => c.Roles).Returns(mockSetRoles.Object);

			// Act
			var result = await _controller.Login(model) as ViewResult;

			// Assert
			Assert.NotNull(result);
			Assert.Null(result.ViewName); // Ensure it returns the default view
			Assert.Equal("Tai khoan khong ton tai", _controller.ViewBag.ErrorMessage);
		}

		private static Mock<DbSet<T>> CreateDbSetMock<T>(List<T> data) where T : class
		{
			var mockSet = new Mock<DbSet<T>>();
			mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.AsQueryable().Provider);
			mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.AsQueryable().Expression);
			mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.AsQueryable().ElementType);
			mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.AsQueryable().GetEnumerator());
			return mockSet;
		}
	}
}
