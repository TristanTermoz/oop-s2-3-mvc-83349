using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Data;
using VgcCollege.Models;

namespace VgcCollege.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly Microsoft.AspNetCore.Identity.UserManager<Microsoft.AspNetCore.Identity.IdentityUser> _userManager;
        private readonly Microsoft.AspNetCore.Identity.RoleManager<Microsoft.AspNetCore.Identity.IdentityRole> _roleManager;

        public AdminController(ApplicationDbContext db,
            Microsoft.AspNetCore.Identity.UserManager<Microsoft.AspNetCore.Identity.IdentityUser> userManager,
            Microsoft.AspNetCore.Identity.RoleManager<Microsoft.AspNetCore.Identity.IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var branches = await _db.Branches.AsNoTracking().ToListAsync();
            var courses = await _db.Courses.Include(c => c.Branch).Include(c => c.Faculty).AsNoTracking().ToListAsync();
            var enrolments = await _db.CourseEnrolments.Include(e => e.Student).Include(e => e.Course).AsNoTracking().ToListAsync();
            var faculty = await _db.FacultyProfiles.AsNoTracking().ToListAsync();
            var students = await _db.StudentProfiles.AsNoTracking().ToListAsync();
            var assignments = await _db.Assignments.Include(a => a.Course).AsNoTracking().ToListAsync();
            var exams = await _db.Exams.Include(e => e.Course).AsNoTracking().ToListAsync();

            var model = new AdminDashboardViewModel
            {
                Branches = branches,
                Courses = courses,
                Enrolments = enrolments,
                FacultyProfiles = faculty,
                StudentProfiles = students,
                Assignments = assignments,
                Exams = exams
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBranch(Branch branch)
        {
            if (!ModelState.IsValid) return RedirectToAction("Index");
            _db.Branches.Add(branch);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBranch(int id)
        {
            var b = await _db.Branches.FindAsync(id);
            if (b != null)
            {
                _db.Branches.Remove(b);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCourse(Course course)
        {
            if (!ModelState.IsValid) return RedirectToAction("Index");
            _db.Courses.Add(course);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var c = await _db.Courses.FindAsync(id);
            if (c != null)
            {
                _db.Courses.Remove(c);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignFaculty(int courseId, int facultyProfileId)
        {
            var c = await _db.Courses.FindAsync(courseId);
            if (c != null)
            {
                c.FacultyProfileId = facultyProfileId;
                await _db.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnrolStudent(int courseId, int studentProfileId)
        {
            var enrol = new CourseEnrolment { CourseId = courseId, StudentProfileId = studentProfileId, EnrolDate = System.DateTime.Today, Status = "Active" };
            _db.CourseEnrolments.Add(enrol);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAssignment(Assignment assignment)
        {
            if (!ModelState.IsValid) return RedirectToAction("Index");
            _db.Assignments.Add(assignment);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateExam(Exam exam)
        {
            if (!ModelState.IsValid) return RedirectToAction("Index");
            _db.Exams.Add(exam);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReleaseExamResults(int examId)
        {
            var exam = await _db.Exams.FindAsync(examId);
            if (exam != null)
            {
                exam.ResultsReleased = true;
                await _db.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}
