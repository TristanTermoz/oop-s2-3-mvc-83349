using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VgcCollege.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace VgcCollege.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class UserAdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public UserAdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
        }

        public async Task<IActionResult> Students()
        {
            var students = await _db.StudentProfiles.AsNoTracking().ToListAsync();
            return View(students);
        }

        public async Task<IActionResult> CreateStudent()
        {
            ViewData["Courses"] = await _db.Courses.AsNoTracking().ToListAsync();
            return View(new Models.StudentCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStudent(Models.StudentCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Courses"] = await _db.Courses.AsNoTracking().ToListAsync();
                return View(model);
            }

            // create identity user
            var user = new IdentityUser { UserName = model.Email, Email = model.Email, EmailConfirmed = true };
            var res = await _userManager.CreateAsync(user, model.Password);
            if (!res.Succeeded)
            {
                // add identity errors to model state so they are shown on the form
                foreach (var err in res.Errors)
                    ModelState.AddModelError(string.Empty, err.Description);

                ViewData["Courses"] = await _db.Courses.AsNoTracking().ToListAsync();
                return View(model);
            }

            // add role
            if (!await _roleManager.RoleExistsAsync("Student")) await _roleManager.CreateAsync(new IdentityRole("Student"));
            await _userManager.AddToRoleAsync(user, "Student");

            // create student profile
            var student = new Models.StudentProfile
            {
                IdentityUserId = user.Id,
                Name = model.Name,
                Email = model.Email,
                Phone = model.Phone,
                Address = model.Address,
                DateOfBirth = model.DateOfBirth ?? DateTime.MinValue,
                StudentNumber = model.StudentNumber
            };
            _db.StudentProfiles.Add(student);
            await _db.SaveChangesAsync();

            // enrol in courses
            if (model.SelectedCourseIds != null)
            {
                foreach (var cid in model.SelectedCourseIds)
                {
                    _db.CourseEnrolments.Add(new Models.CourseEnrolment { CourseId = cid, StudentProfileId = student.Id, EnrolDate = DateTime.Today, Status = "Active" });
                }
                await _db.SaveChangesAsync();
            }

            return RedirectToAction("Students");
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string email, string password, string role)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(role)) return RedirectToAction("Index");
            var user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
            var res = await _userManager.CreateAsync(user, password);
            if (res.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync(role)) await _roleManager.CreateAsync(new IdentityRole(role));
                await _userManager.AddToRoleAsync(user, role);
            }
            return RedirectToAction("Index");
        }
    }
}
