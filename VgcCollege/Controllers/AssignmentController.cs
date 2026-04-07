using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VgcCollege.Data;
using VgcCollege.Models;
using Microsoft.EntityFrameworkCore;

namespace VgcCollege.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AssignmentController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AssignmentController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var assignments = await _db.Assignments.Include(a => a.Course).AsNoTracking().ToListAsync();
            return View(assignments);
        }

        public async Task<IActionResult> Create()
        {
            ViewData["Courses"] = await _db.Courses.AsNoTracking().ToListAsync();
            return View(new AssignmentCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AssignmentCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Courses"] = await _db.Courses.AsNoTracking().ToListAsync();
                return View(model);
            }
            var assignment = new Assignment { Title = model.Title, CourseId = model.CourseId ?? 0, MaxScore = model.MaxScore, DueDate = model.DueDate };
            _db.Assignments.Add(assignment);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var assignment = await _db.Assignments.FindAsync(id);
            if (assignment == null) return NotFound();
            ViewData["Courses"] = await _db.Courses.AsNoTracking().ToListAsync();
            return View(assignment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Assignment model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Courses"] = await _db.Courses.AsNoTracking().ToListAsync();
                return View(model);
            }

            var existing = await _db.Assignments.FindAsync(model.Id);
            if (existing == null) return NotFound();

            existing.Title = model.Title;
            existing.MaxScore = model.MaxScore;
            existing.DueDate = model.DueDate;
            existing.CourseId = model.CourseId;

            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var a = await _db.Assignments.FindAsync(id);
            if (a != null)
            {
                _db.Assignments.Remove(a);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}
