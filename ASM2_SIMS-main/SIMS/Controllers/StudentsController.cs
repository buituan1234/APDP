using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIMS.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Azure.Identity;
using NuGet.DependencyResolver;

namespace SIMS.Controllers
{
    public class StudentsController : Controller
    {
        private readonly SimsContext _context;

        public StudentsController(SimsContext context)
        {
            _context = context;
        }

        // GET: Students
        public async Task<IActionResult> Index()
        {
            var simsContext = _context.Students.Include(s => s.Role);
            return View(await simsContext.ToListAsync());
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.Role)
                .FirstOrDefaultAsync(m => m.StudentId == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleId");
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StudentId,UserName,Password,FullName,Email,DateOfBirth,Address,RoleId")] Student student)
        {
            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleId", student.RoleId);
            return View(student);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleId", student.RoleId);
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StudentId,UserName,Password,FullName,Email,DateOfBirth,Address,RoleId")] Student student)
        {
            if (id != student.StudentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.StudentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleId", student.RoleId);
            return View(student);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.Role)
                .FirstOrDefaultAsync(m => m.StudentId == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.StudentId == id);
        }

        [HttpGet]
        [Authorize(Roles = "Student")]
        public IActionResult HomePage()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Student model)
        {
            if (ModelState.IsValid)
            {
                var student = _context.Students.FirstOrDefault(a => a.UserName == model.UserName && a.Password == model.Password);

                if (student == null)
                {
                    ViewBag.ErrorMessage = "Tai khoan khong ton tai";
                    return View();
                }
                else
                {
                    var role = _context.Roles.FirstOrDefault(r => r.RoleId == student.RoleId);
                    var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, student.StudentId.ToString()),
                    new Claim(ClaimTypes.Role, "Student"),
                    new Claim("Email", student.Email),
                    new Claim("Address", student.Address),

                };
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    await HttpContext.SignInAsync(claimsPrincipal);

                    return RedirectToAction("HomePage", "Students");
                }
            }
            return View();


        }
        public async Task<IActionResult> CreateStudent()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login", "Admins");

        }

		public async Task<IActionResult> logout()
		{
			await HttpContext.SignOutAsync();
			return RedirectToAction("Login", "Students");

		}
   
        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(int? id, Student model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // lấy dữ liệu thông tin khi đăng nhập thành công
            if (userId == null)
            {
                return RedirectToAction("Login", "Students");
            }

            var StudentID = int.Parse(userId);
            if (StudentID != model.StudentId)
            {
                return NotFound();
            }
            try
            {
                var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == StudentID);
                if (student == null)
                {
                    return NotFound();
                }
                student.FullName = model.FullName;
                student.Address = model.Address ?? string.Empty; // Xử lý giá trị null
                student.DateOfBirth = model.DateOfBirth;
                student.Email = model.Email ?? string.Empty;     // Xử lý giá trị null

                _context.Update(student);
                await _context.SaveChangesAsync();
                return RedirectToAction("HomePage", "Students");
            }
            catch (DbUpdateConcurrencyException)
            {
                ModelState.AddModelError("", "Unable to save changes. The student was updated or deleted by another user.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred while saving changes: {ex.Message}");
            }
            // Nếu có lỗi xảy ra, cần điền lại dữ liệu cho model để hiển thị lại view
            ViewBag.RoleId = new SelectList(_context.Roles, "RoleId", "RoleName", model.RoleId);

            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return RedirectToAction("Login", "Teachers");
            }

            var StudentID = int.Parse(userId);
            var Students = await _context.Students
                .FirstOrDefaultAsync(s => s.StudentId == StudentID);

            if (Students == null)
            {
                return NotFound();
            }

            var model = new Student
            {
                StudentId = Students.StudentId,
                FullName = Students.FullName,
                Address = Students.Address ?? string.Empty,
                DateOfBirth = Students.DateOfBirth,
                Email = Students.Email ?? string.Empty,
                UserName = Students.UserName,
            };
            ViewBag.RoleId = new SelectList(_context.Roles, "RoleId", "RoleName", Students.RoleId);
            return View(model);
        }
    }

}

