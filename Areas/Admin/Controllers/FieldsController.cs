using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project.Areas.Admin.Models;
using project.Models; // 👈 Thêm dòng này để nhận ra DataContext

namespace project.Areas.Admin.Models
{
    [Area("Admin")]
    public class FieldsController : Controller
    {
        private readonly DataContext _context;

        public FieldsController(DataContext context)
        {
            _context = context;
        }

        // GET: Admin/Fields
        public async Task<IActionResult> Index()
        {
            var fields = await _context.tblFields
                .Include(f => f.Teacher)
                .ToListAsync();

            return View(fields);
        }

        // GET: Admin/Fields/Create
        public IActionResult Create()
        {
            ViewBag.Teachers = _context.Teachers.ToList();
            return View();
        }

        // POST: Admin/Fields/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(tblFields field)
        {
            if (ModelState.IsValid)
            {
                _context.Add(field);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Teachers = _context.Teachers.ToList();
            return View(field);
        }

        // GET: Admin/Fields/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var field = await _context.tblFields.FindAsync(id);
            if (field == null)
                return NotFound();

            ViewBag.Teachers = _context.Teachers.ToList();
            return View(field);
        }

        // POST: Admin/Fields/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, tblFields field)
        {
            if (id != field.FieldID)
                return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(field);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Teachers = _context.Teachers.ToList();
            return View(field);
        }

        // GET: Admin/Fields/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var field = await _context.tblFields.FindAsync(id);
            if (field == null)
                return NotFound();

            _context.tblFields.Remove(field);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
