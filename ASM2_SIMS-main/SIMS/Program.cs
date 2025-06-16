using SIMS.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace SIMS
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddDbContext<SimsContext>(option => {
				option.UseSqlServer(builder.Configuration.GetConnectionString("SIMSContext"));
			});
			builder.Services.AddControllersWithViews();
			builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme) /// copy
			.AddCookie(options =>
			{
				//options.LoginPath = "/Students/LoginStudent";
				options.LoginPath = "/Admins/Login";
				options.AccessDeniedPath = "/AccessDenied";
			});
			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();
			app.UseAuthentication(); //// copy
			app.UseAuthorization();

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Admins}/{action=Login}/{id?}");

			app.Run();
		}
	}
}
