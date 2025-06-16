using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIMS.Controllers;
using SIMS.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SIMS.Tests
{
	public class ClassesControllerTests
	{
		private readonly SimsContext _context;
		private readonly ClassesController _controller;

		public ClassesControllerTests()
		{
			// Configure the in-memory database
			var options = new DbContextOptionsBuilder<SimsContext>()
						.UseInMemoryDatabase(databaseName: "TestDatabaseclass")
						.Options;

			// Initialize the database context
			_context = new SimsContext(options);

			// Initialize the controller with the context
			_controller = new ClassesController(_context);

			// Seed the database with initial data if needed
			SeedDatabase();
		}

		private void SeedDatabase()
		{
			// Remove existing data from both Classes and Courses tables
			_context.Classes.RemoveRange(_context.Classes);
			_context.Courses.RemoveRange(_context.Courses);
			_context.SaveChanges();

			// Add courses with unique CourseId
			_context.Courses.AddRange(
				new Course { CourseId = 1, CourseName = "Mathematics", Description = "Math course" },
				new Course { CourseId = 2, CourseName = "Science", Description = "Science course" }
			);

			// Add classes with unique ClassId
			_context.Classes.AddRange(
				new Class { ClassId = 1, ClassName = "Math Class A", CourseId = 1 },
				new Class { ClassId = 2, ClassName = "Science Class A", CourseId = 2 }
			);

			_context.SaveChanges();
		}



		[Fact]
		public async Task Index_ReturnsAViewResult_WithAListOfClasses()
		{
			// Act
			var result = await _controller.Index();

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);
			var model = Assert.IsAssignableFrom<IEnumerable<Class>>(viewResult.Model);
			Assert.Equal(2, model.Count());
		}

		[Fact]
		public async Task Create_PostValidModel_RedirectsToIndex()
		{
			// Arrange
			var @class = new Class { ClassId = 3, ClassName = "English Class", CourseId = 1 };

			// Act
			var result = await _controller.Create(@class);

			// Assert
			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Index", redirectToActionResult.ActionName);
			Assert.True(_context.Classes.Any(c => c.ClassName == "English Class"));
		}

		[Fact]
		public async Task Edit_PostInvalidId_ReturnsNotFound()
		{
			// Arrange
			var invalidClassId = 99;
			var classToUpdate = new Class { ClassId = 99, ClassName = "Test Class", CourseId = 1 };

			// Act
			var result = await _controller.Edit(invalidClassId, classToUpdate);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public async Task Delete_Confirmed_ReturnsRedirectToIndex()
		{
			// Arrange
			var classIdToDelete = 1;

			// Act
			var result = await _controller.DeleteConfirmed(classIdToDelete);

			// Assert
			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Index", redirectToActionResult.ActionName);
			Assert.False(_context.Classes.Any(c => c.ClassId == classIdToDelete));
		}

		[Fact]
		public async Task ViewClass_ReturnsAViewResult_WithAListOfClasses()
		{
			// Act
			var result = await _controller.ViewClass();

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);
			var model = Assert.IsAssignableFrom<IEnumerable<Class>>(viewResult.Model);
			Assert.Equal(2, model.Count());
		}

		// Cleanup
		public void Dispose()
		{
			_context.Dispose();
		}


	}
}
