using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project.Models;
using System.Linq;
using System.Collections.Generic;

namespace project.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class FieldsController : Controller
    {
        private readonly DataContext _context;

        public FieldsController(DataContext context)
        {
            _context = context;
        }

        // GET: Index
        public IActionResult Index()
        {
            var fields = _context.Fields
                .Include(f => f.tblTeacherFields)
                .ThenInclude(tf => tf.Teacher)
                .ToList();
            return View(fields);
        }

        // GET: Create
        public IActionResult Create()
        {
            ViewBag.Teachers = _context.Teachers.ToList() ?? new List<tblTeachers>();
            return View();
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(tblFields field, int[] selectedTeachers)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Teachers = _context.Teachers.ToList() ?? new List<tblTeachers>();
                return View(field);
            }

            // Thêm giáo viên phụ trách
            if (selectedTeachers != null)
            {
                foreach (var teacherId in selectedTeachers)
                {
                    field.tblTeacherFields.Add(new tblTeacherFields { TeacherID = teacherId });
                }
            }

            _context.Fields.Add(field);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // GET: Edit
        public IActionResult Edit(int id)
        {
            var field = _context.Fields
                                .Include(f => f.tblTeacherFields)
                                .FirstOrDefault(f => f.FieldID == id);

            if (field == null) return NotFound();

            ViewBag.Teachers = _context.Teachers.ToList() ?? new List<tblTeachers>();
            ViewBag.SelectedTeachers = field.tblTeacherFields.Select(tf => tf.TeacherID).ToList();

            return View(field);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(tblFields field, int[] selectedTeachers)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Teachers = _context.Teachers.ToList() ?? new List<tblTeachers>();
                ViewBag.SelectedTeachers = selectedTeachers ?? new int[] { };
                return View(field);
            }

            var existingField = _context.Fields
                                        .Include(f => f.tblTeacherFields)
                                        .FirstOrDefault(f => f.FieldID == field.FieldID);

            if (existingField != null)
            {
                existingField.FieldName = field.FieldName;
                existingField.Description = field.Description;
                existingField.Status = field.Status;
                existingField.StartDate = field.StartDate;
                existingField.EndDate = field.EndDate;

                // Xóa hết giáo viên cũ
                existingField.tblTeacherFields.Clear();

                // Thêm giáo viên mới
                if (selectedTeachers != null)
                {
                    foreach (var teacherId in selectedTeachers)
                    {
                        existingField.tblTeacherFields.Add(new tblTeacherFields { TeacherID = teacherId });
                    }
                }

                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Delete
        public IActionResult Delete(int id)
        {
            var field = _context.Fields
                .Include(f => f.tblTeacherFields)
                .ThenInclude(tf => tf.Teacher)
                .FirstOrDefault(f => f.FieldID == id);

            if (field == null) return NotFound();
            return View(field);
        }

        // POST: Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var field = _context.Fields
                                .Include(f => f.tblTeacherFields)
                                .FirstOrDefault(f => f.FieldID == id);

            if (field != null)
            {
                _context.Fields.Remove(field);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
