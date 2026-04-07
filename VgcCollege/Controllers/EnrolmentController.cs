using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VgcCollege.Data;
using VgcCollege.Models;
using Microsoft.EntityFrameworkCore;

namespace VgcCollege.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class EnrolmentController : Controller
    {
        private readonly ApplicationDbContext _db;

        public EnrolmentController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var enrolments = await _db.CourseEnrolments.Include(e => e.Student).Include(e => e.Course).AsNoTracking().ToListAsync();
            return View(enrolments);
        }

        public async Task<IActionResult> Create()
        {
            ViewData["Courses"] = await _db.Courses.AsNoTracking().ToListAsync();
            ViewData["Students"] = await _db.StudentProfiles.AsNoTracking().ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int courseId, int studentId)
        {
            var enrol = new CourseEnrolment { CourseId = courseId, StudentProfileId = studentId, EnrolDate = System.DateTime.Today, Status = "Active" };
            _db.CourseEnrolments.Add(enrol);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
