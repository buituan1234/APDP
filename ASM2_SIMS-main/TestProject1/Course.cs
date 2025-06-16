using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIMS.Controllers;
using SIMS.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System.Net.Http;
using NuGet.DependencyResolver;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;



namespace SIMS.Tests
{
	public class CoursesControllerTests
	{
		private readonly SimsContext _context;
		private readonly CoursesController _controller;

		public CoursesControllerTests()
		{
			// Configure the in-memory database
			var options = new DbContextOptionsBuilder<SimsContext>()
						.UseInMemoryDatabase(databaseName: "TestDatabase") // Ensure a unique name
						.Options;

			// Initialize the database context
			_context = new SimsContext(options);

			// Initialize the controller with the context
			_controller = new CoursesController(_context);

			// Seed the database with initial data if needed
			SeedDatabase();
		}

		private void SeedDatabase()
		{
			// Clear existing data to avoid duplicate key issues
			_context.Courses.RemoveRange(_context.Courses);
			_context.SaveChanges();

			// Add courses with unique CourseId
			_context.Courses.AddRange(
				new Course { CourseId = 1, CourseName = "Mathematics", Description = "Math course" },
				new Course { CourseId = 2, CourseName = "Science", Description = "Science course" }
			);
			_context.SaveChanges();
		}


		[Fact]
		public async Task Index_ReturnsAViewResult_WithAListOfCourses()
		{
			// Act
			var result = await _controller.Index();

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);
			var model = Assert.IsAssignableFrom<IEnumerable<Course>>(viewResult.Model);
			Assert.Equal(2, model.Count());
		}

		[Fact]
		public async Task Create_PostValidModel_RedirectsToIndex()
		{
			// Arrange
			var course = new Course { CourseId = 3, CourseName = "English", Description = "English course" };

			// Act
			var result = await _controller.Create(course);

			// Assert
			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Index", redirectToActionResult.ActionName);
			Assert.True(_context.Courses.Any(c => c.CourseName == "English"));
		}

		[Fact]
		public async Task Edit_PostInvalidId_ReturnsNotFound()
		{
			// Arrange
			var invalidCourseId = 99;
			var courseToUpdate = new Course { CourseId = 99, CourseName = "Test", Description = "Test Desc" };

			// Act
			var result = await _controller.Edit(invalidCourseId, courseToUpdate);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public async Task Delete_Confirmed_ReturnsRedirectToIndex()
		{
			// Arrange
			var courseIdToDelete = 1;

			// Act
			var result = await _controller.DeleteConfirmed(courseIdToDelete);

			// Assert
			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Index", redirectToActionResult.ActionName);
			Assert.False(_context.Courses.Any(c => c.CourseId == courseIdToDelete));
		}

		// Cleanup
		public void Dispose()
		{
			_context.Dispose();
		}
	}


}
