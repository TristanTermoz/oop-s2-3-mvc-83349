using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Data;
using VgcCollege.Models;
using Microsoft.AspNetCore.Identity;

namespace VgcCollege.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public StudentController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var student = await _db.StudentProfiles.Include(s => s.Enrolments).ThenInclude(e => e.Course).FirstOrDefaultAsync(s => s.IdentityUserId == user.Id);
            if (student == null) return View(new StudentDashboardViewModel());

            var enrolments = await _db.CourseEnrolments.Where(e => e.StudentProfileId == student.Id).Include(e => e.Course).AsNoTracking().ToListAsync();
            var assignmentResults = await _db.AssignmentResults.Where(r => r.StudentProfileId == student.Id).Include(r => r.Assignment).AsNoTracking().ToListAsync();
            var examResults = await _db.ExamResults.Where(r => r.StudentProfileId == student.Id).Include(r => r.Exam).AsNoTracking().ToListAsync();

            var model = new StudentDashboardViewModel
            {
                Student = student,
                Enrolments = enrolments,
                AssignmentResults = assignmentResults,
                ExamResults = examResults.Where(er => er.Exam.ResultsReleased).ToList()
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            var student = await _db.StudentProfiles.FirstOrDefaultAsync(s => s.IdentityUserId == user.Id);
            if (student == null) return NotFound();
            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(StudentProfile model)
        {
            var user = await _userManager.GetUserAsync(User);
            var student = await _db.StudentProfiles.FirstOrDefaultAsync(s => s.IdentityUserId == user.Id);
            if (student == null) return NotFound();

            student.Name = model.Name;
            student.Email = model.Email;
            student.Phone = model.Phone;
            student.Address = model.Address;
            student.DateOfBirth = model.DateOfBirth;

            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
