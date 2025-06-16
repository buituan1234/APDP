using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using SIMS.Controllers;
using SIMS.Models;

namespace SIMS.Tests
{
	public class ScoresControllerTests
	{
		private DbContextOptions<SimsContext> _dbContextOptions;

		public ScoresControllerTests()
		{
			_dbContextOptions = new DbContextOptionsBuilder<SimsContext>()
				.UseInMemoryDatabase(databaseName: "TestDatabaseScore")
				.Options;
		}

		private async Task SeedDatabase()
		{
			// Tạo mới DbContext để tránh trùng lặp dữ liệu
			using (var context = new SimsContext(_dbContextOptions))
			{
				// Xóa tất cả dữ liệu hiện có
				context.Scores.RemoveRange(context.Scores);
				context.Enrollments.RemoveRange(context.Enrollments);

				// Thêm dữ liệu mới
				context.Enrollments.AddRange(
					new Enrollment { EnrollmentId = 1 },
					new Enrollment { EnrollmentId = 2 }
				);
				context.Scores.AddRange(
					new Score { ScoreId = 1, EnrollmentId = 1, Score1 = 85 },
					new Score { ScoreId = 2, EnrollmentId = 2, Score1 = 90 }
				);
				await context.SaveChangesAsync();
			}
		}

		[Fact]
		public async Task Create_PostAddsNewScore_WhenModelStateIsValid()
		{
			// Arrange
			using (var context = new SimsContext(_dbContextOptions))
			{
				var controller = new ScoresController(context);
				var score = new Score { EnrollmentId = 3, Score1 = 75 };

				// Act
				var result = await controller.Create(score);

				// Assert
				var redirectResult = Assert.IsType<RedirectToActionResult>(result);
				Assert.Equal("Index", redirectResult.ActionName);

				using (var assertContext = new SimsContext(_dbContextOptions))
				{
					Assert.Equal(3, assertContext.Scores.Count());
				}
			}
		}

		[Fact]
		public async Task Details_ReturnsViewResult_WhenScoreIsFound()
		{
			// Arrange
			await SeedDatabase();

			using (var context = new SimsContext(_dbContextOptions))
			{
				var controller = new ScoresController(context);

				// Act
				var result = await controller.Details(1);

				// Assert
				var viewResult = Assert.IsType<ViewResult>(result);
				var model = Assert.IsType<Score>(viewResult.Model);
				Assert.Equal(1, model.ScoreId);
			}
		}

		[Fact]
		public async Task Edit_PostUpdatesScore_WhenModelStateIsValid()
		{
			// Arrange
			await SeedDatabase();

			using (var context = new SimsContext(_dbContextOptions))
			{
				var controller = new ScoresController(context);
				var updatedScore = new Score { ScoreId = 1, EnrollmentId = 1, Score1 = 95 };

				// Act
				var result = await controller.Edit(1, updatedScore);

				// Assert
				var redirectResult = Assert.IsType<RedirectToActionResult>(result);
				Assert.Equal("Index", redirectResult.ActionName);

				using (var assertContext = new SimsContext(_dbContextOptions))
				{
					var score = await assertContext.Scores.FindAsync(1);
					Assert.Equal(95, score.Score1);
				}
			}
		}

		[Fact]
		public async Task Delete_RemovesScore_WhenScoreIsFound()
		{
			// Arrange
			await SeedDatabase();

			using (var context = new SimsContext(_dbContextOptions))
			{
				var controller = new ScoresController(context);

				// Act
				var result = await controller.DeleteConfirmed(1);

				// Assert
				var redirectResult = Assert.IsType<RedirectToActionResult>(result);
				Assert.Equal("Index", redirectResult.ActionName);

				using (var assertContext = new SimsContext(_dbContextOptions))
				{
					Assert.Null(await assertContext.Scores.FindAsync(1));
				}
			}
		}
	}
}
