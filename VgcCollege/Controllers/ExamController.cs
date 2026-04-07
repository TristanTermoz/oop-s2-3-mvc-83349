using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VgcCollege.Data;
using VgcCollege.Models;
using Microsoft.EntityFrameworkCore;

namespace VgcCollege.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class ExamController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ExamController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var exams = await _db.Exams.Include(e => e.Course).AsNoTracking().ToListAsync();
            return View(exams);
        }

        public async Task<IActionResult> Create()
        {
            ViewData["Courses"] = await _db.Courses.AsNoTracking().ToListAsync();
            return View(new ExamCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExamCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Courses"] = await _db.Courses.AsNoTracking().ToListAsync();
                return View(model);
            }
            var exam = new Exam { Title = model.Title, CourseId = model.CourseId ?? 0, Date = model.Date, MaxScore = model.MaxScore };
            _db.Exams.Add(exam);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Release(int id)
        {
            var exam = await _db.Exams.FindAsync(id);
            if (exam != null)
            {
                exam.ResultsReleased = true;
                await _db.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}
