using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VgcCollege.Data;
using VgcCollege.Models;
using Microsoft.EntityFrameworkCore;

namespace VgcCollege.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class CourseController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CourseController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var courses = await _db.Courses.Include(c => c.Branch).Include(c => c.Faculty).AsNoTracking().ToListAsync();
            return View(courses);
        }

        public async Task<IActionResult> Create()
        {
            ViewData["Branches"] = await _db.Branches.AsNoTracking().ToListAsync();
            return View(new CourseCreateViewModel { StartDate = DateTime.Today, EndDate = DateTime.Today.AddMonths(6) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Branches"] = await _db.Branches.AsNoTracking().ToListAsync();
                return View(model);
            }

            var course = new Course { Name = model.Name, BranchId = model.BranchId ?? 0, StartDate = model.StartDate, EndDate = model.EndDate };
            _db.Courses.Add(course);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var course = await _db.Courses.FindAsync(id);
            if (course == null) return NotFound();
            ViewData["Branches"] = await _db.Branches.AsNoTracking().ToListAsync();
            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Course model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Branches"] = await _db.Branches.AsNoTracking().ToListAsync();
                return View(model);
            }
            var existing = await _db.Courses.FindAsync(model.Id);
            if (existing == null) return NotFound();
            existing.Name = model.Name;
            existing.BranchId = model.BranchId;
            existing.StartDate = model.StartDate;
            existing.EndDate = model.EndDate;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var course = await _db.Courses.FindAsync(id);
            if (course != null)
            {
                _db.Courses.Remove(course);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
