using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Data;
using VgcCollege.Models;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Collections.Generic;

namespace VgcCollege.Controllers
{
    [Authorize(Roles = "Faculty")]
    public class FacultyController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public FacultyController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var facultyProfile = await _db.FacultyProfiles.FirstOrDefaultAsync(f => f.IdentityUserId == user.Id);
            if (facultyProfile == null) return View(new FacultyDashboardViewModel());

            var courses = await _db.Courses.Where(c => c.FacultyProfileId == facultyProfile.Id)
                .Include(c => c.Enrolments).ThenInclude(e => e.Student)
                .AsNoTracking().ToListAsync();

            var enrolments = await _db.CourseEnrolments.Where(e => e.Course.FacultyProfileId == facultyProfile.Id)
                .Include(e => e.Student).Include(e => e.Course)
                .AsNoTracking().ToListAsync();

            var results = await _db.AssignmentResults.Where(r => r.Assignment.Course.FacultyProfileId == facultyProfile.Id)
                .Include(r => r.Student).Include(r => r.Assignment)
                .AsNoTracking().ToListAsync();

            var model = new FacultyDashboardViewModel
            {
                Courses = courses,
                Enrolments = enrolments,
                AssignmentResults = results
            };

            return View(model);
        }

        // List students enrolled in faculty's courses
        public async Task<IActionResult> Students()
        {
            var user = await _userManager.GetUserAsync(User);
            var facultyProfile = await _db.FacultyProfiles.FirstOrDefaultAsync(f => f.IdentityUserId == user.Id);
            if (facultyProfile == null) return Forbid();

            var enrolments = await _db.CourseEnrolments
                .Where(e => e.Course.FacultyProfileId == facultyProfile.Id)
                .Include(e => e.Student)
                .Include(e => e.Course)
                .AsNoTracking()
                .ToListAsync();

            return View(enrolments);
        }

        // Show student contact details and results for students the faculty teaches
        public async Task<IActionResult> StudentDetails(int id)
        {
            // id = studentProfileId
            var user = await _userManager.GetUserAsync(User);
            var facultyProfile = await _db.FacultyProfiles.FirstOrDefaultAsync(f => f.IdentityUserId == user.Id);
            if (facultyProfile == null) return Forbid();

            // ensure faculty teaches at least one course this student is enrolled in
            var related = await _db.CourseEnrolments.AnyAsync(e => e.StudentProfileId == id && e.Course.FacultyProfileId == facultyProfile.Id);
            if (!related) return Forbid();

            var student = await _db.StudentProfiles.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
            if (student == null) return NotFound();

            var assignmentResults = await _db.AssignmentResults
                .Where(r => r.StudentProfileId == id && r.Assignment.Course.FacultyProfileId == facultyProfile.Id)
                .Include(r => r.Assignment)
                .AsNoTracking()
                .ToListAsync();

            var examResults = await _db.ExamResults
                .Where(r => r.StudentProfileId == id && r.Exam.Course.FacultyProfileId == facultyProfile.Id)
                .Include(r => r.Exam)
                .AsNoTracking()
                .ToListAsync();

            ViewData["AssignmentResults"] = assignmentResults;
            ViewData["ExamResults"] = examResults;

            return View(student);
        }

        [HttpGet]
        public async Task<IActionResult> CreateAssignmentResult(int assignmentId, int studentId)
        {
            var user = await _userManager.GetUserAsync(User);
            var facultyProfile = await _db.FacultyProfiles.FirstOrDefaultAsync(f => f.IdentityUserId == user.Id);
            if (facultyProfile == null) return Forbid();

            var assignment = await _db.Assignments.Include(a => a.Course).FirstOrDefaultAsync(a => a.Id == assignmentId);
            if (assignment == null || assignment.Course.FacultyProfileId != facultyProfile.Id) return Forbid();

            var enrolled = await _db.CourseEnrolments.AnyAsync(e => e.CourseId == assignment.CourseId && e.StudentProfileId == studentId);
            if (!enrolled) return Forbid();

            var model = new AssignmentResult { AssignmentId = assignmentId, StudentProfileId = studentId };
            ViewData["AssignmentTitle"] = assignment.Title;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAssignmentResult(AssignmentResult model)
        {
            var user = await _userManager.GetUserAsync(User);
            var facultyProfile = await _db.FacultyProfiles.FirstOrDefaultAsync(f => f.IdentityUserId == user.Id);
            if (facultyProfile == null) return Forbid();

            var assignment = await _db.Assignments.Include(a => a.Course).FirstOrDefaultAsync(a => a.Id == model.AssignmentId);
            if (assignment == null || assignment.Course.FacultyProfileId != facultyProfile.Id) return Forbid();

            var enrolled = await _db.CourseEnrolments.AnyAsync(e => e.CourseId == assignment.CourseId && e.StudentProfileId == model.StudentProfileId);
            if (!enrolled) return Forbid();

            // update assignment title if provided
            var title = Request.Form["Assignment.Title"].ToString();
            if (!string.IsNullOrWhiteSpace(title))
            {
                var dbAssignment = await _db.Assignments.FindAsync(model.AssignmentId);
                if (dbAssignment != null)
                {
                    dbAssignment.Title = title;
                }
            }

            _db.AssignmentResults.Add(model);
            await _db.SaveChangesAsync();
            return RedirectToAction("StudentDetails", new { id = model.StudentProfileId });
        }

        [HttpGet]
        public async Task<IActionResult> EditAssignmentResult(int id)
        {
            var result = await _db.AssignmentResults.Include(r => r.Assignment).ThenInclude(a => a.Course).FirstOrDefaultAsync(r => r.Id == id);
            if (result == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            var facultyProfile = await _db.FacultyProfiles.FirstOrDefaultAsync(f => f.IdentityUserId == user.Id);
            if (facultyProfile == null || result.Assignment.Course.FacultyProfileId != facultyProfile.Id) return Forbid();

            return View(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAssignmentResult(AssignmentResult model)
        {
            var existing = await _db.AssignmentResults.Include(r => r.Assignment).ThenInclude(a => a.Course).FirstOrDefaultAsync(r => r.Id == model.Id);
            if (existing == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            var facultyProfile = await _db.FacultyProfiles.FirstOrDefaultAsync(f => f.IdentityUserId == user.Id);
            if (facultyProfile == null || existing.Assignment.Course.FacultyProfileId != facultyProfile.Id) return Forbid();

            // allow updating assignment title
            var title = Request.Form["Assignment.Title"].ToString();
            if (!string.IsNullOrWhiteSpace(title))
            {
                var a = await _db.Assignments.FindAsync(existing.AssignmentId);
                if (a != null) a.Title = title;
            }

            existing.Score = model.Score;
            existing.Feedback = model.Feedback;
            await _db.SaveChangesAsync();

            return RedirectToAction("StudentDetails", new { id = existing.StudentProfileId });
        }

        public async Task<IActionResult> ExamResults()
        {
            var user = await _userManager.GetUserAsync(User);
            var facultyProfile = await _db.FacultyProfiles.FirstOrDefaultAsync(f => f.IdentityUserId == user.Id);
            if (facultyProfile == null) return Forbid();

            var results = await _db.ExamResults
                .Where(r => r.Exam.Course.FacultyProfileId == facultyProfile.Id)
                .Include(r => r.Student)
                .Include(r => r.Exam)
                .AsNoTracking()
                .ToListAsync();

            return View(results);
        }

        public async Task<IActionResult> Gradebook()
        {
            var user = await _userManager.GetUserAsync(User);
            var facultyProfile = await _db.FacultyProfiles.FirstOrDefaultAsync(f => f.IdentityUserId == user.Id);
            if (facultyProfile == null) return Forbid();

            var results = await _db.AssignmentResults
                .Where(r => r.Assignment.Course.FacultyProfileId == facultyProfile.Id)
                .Include(r => r.Student)
                .Include(r => r.Assignment)
                .AsNoTracking()
                .ToListAsync();

            return View(results);
        }
    }
}
