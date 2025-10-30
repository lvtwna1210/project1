using Microsoft.AspNetCore.Mvc;
using project.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace project.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class StudentsController : Controller
    {
        private readonly DataContext _context;

        public StudentsController(DataContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var student = _context.Students.ToList();
            return View(student);
        }

        //xóa
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();
            var mn = _context.Students.Find(id);
            if (mn == null)
                return NotFound();
            return View(mn); // Nếu muốn xác nhận trước khi xóa
        }
        [HttpPost]

        public IActionResult Delete(int id)
        {
            var delstudent = _context.Students.Find(id);
            if (delstudent == null)
                return NotFound();
            _context.Students.Remove(delstudent);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        
        //Thêm
        public IActionResult Create()
        {
            var mnList = _context.Students
                        .Select(m => new SelectListItem
                        {
                            Text = m.FullName,
                            Value = m.StudentID.ToString()
                        }).ToList();
            mnList.Insert(0, new SelectListItem
            {
                Text = "--- Chọn học sinh ---",
                Value = "0"
            });
            ViewBag.StudentList = mnList;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(tblStudents student)
        {
            if (ModelState.IsValid)
            {
                _context.Students.Add(student);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(student);
        }
        //Sửa
        public IActionResult Edit(int id)
        {
            var Student = _context.Students.FirstOrDefault(t => t.StudentID == id);
            if (Student == null)
            {
                return NotFound();
            }
            return View(Student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(tblStudents student)
        {
            if (ModelState.IsValid)
            {
                _context.Update(student);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }
    }
}
