using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
using SIMS.Controllers;
using SIMS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Routing;

namespace SIMS.Tests
{
	public class StudentsControllerTests
	{
		private readonly Mock<SimsContext> _mockContext;
		private readonly StudentsController _controller;

		public StudentsControllerTests()
		{
			_mockContext = new Mock<SimsContext>();
			_controller = new StudentsController(_mockContext.Object);

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
				RouteData = new RouteData(), // Khởi tạo RouteData
				ActionDescriptor = new ControllerActionDescriptor() // Khởi tạo đúng loại ActionDescriptor
			};
		}

		[Fact]
		public async Task Login_ValidStudent_RedirectsToHomePage()
		{
			// Arrange
			var student = new Student { StudentId = 1, UserName = "student", Password = "password", Email = "student@example.com", Address = "123 Main St", RoleId = 1 };
			var role = new Role { RoleId = 1 };

			var mockSetStudents = CreateDbSetMock(new List<Student> { student });
			var mockSetRoles = CreateDbSetMock(new List<Role> { role });

			_mockContext.Setup(c => c.Students).Returns(mockSetStudents.Object);
			_mockContext.Setup(c => c.Roles).Returns(mockSetRoles.Object);

			var model = new Student { UserName = "student", Password = "password" };

			// Act
			var result = await _controller.Login(model) as RedirectToActionResult;

			// Assert
			Assert.NotNull(result);
			Assert.Equal("HomePage", result.ActionName);
			Assert.Equal("Students", result.ControllerName);
		}

		[Fact]
		public async Task Login_InvalidStudent_ReturnsViewWithErrorMessage()
		{
			// Arrange
			var model = new Student { UserName = "wrong", Password = "wrong" };

			var mockSetStudents = CreateDbSetMock(new List<Student>());
			var mockSetRoles = CreateDbSetMock(new List<Role>());

			_mockContext.Setup(c => c.Students).Returns(mockSetStudents.Object);
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
