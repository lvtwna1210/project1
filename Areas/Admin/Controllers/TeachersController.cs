using System.Reflection.Emit;
using System.Reflection.PortableExecutable;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using project.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace project.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TeachersController : Controller
    {
        private readonly DataContext _context;
        public TeachersController(DataContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var mnList = _context.Teachers.OrderBy(m => m.TeacherID).ToList();
            return View(mnList);
        }

        //xóa
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();
            var mn = _context.Teachers.Find(id);
            if (mn == null)
                return NotFound();
            return View(mn);
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var delTeachers = _context.Teachers.Find(id);
            if (delTeachers == null)
                return NotFound();
            _context.Teachers.Remove(delTeachers);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        //Thêm
        public IActionResult Create()
        {
            var mnList = _context.Teachers
                        .Select(m => new SelectListItem
                        {
                            Text = m.FullName,
                            Value = m.TeacherID.ToString()
                        }).ToList();
            mnList.Insert(0, new SelectListItem
            {
                Text = "--- Chọn giáo viên ---",
                Value = "0"
            });
            ViewBag.TeacherList = mnList;
            return View();
        }
        [HttpPost]
        public IActionResult Create(tblTeachers teacher)
        {
            if (ModelState.IsValid)
            {
                _context.Teachers.Add(teacher);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(teacher);
        }
        //Sửa
        public IActionResult Edit(int id)
        {
            var teacher = _context.Teachers.FirstOrDefault(t => t.TeacherID == id);
            if (teacher == null)
            {
                return NotFound();
            }
            return View(teacher);
        }

        // POST: Admin/Teachers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(tblTeachers teacher)
        {
            if (ModelState.IsValid)
            {
                _context.Update(teacher);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(teacher);
        }

    }
}using System.Reflection.Emit;
using System.Reflection.PortableExecutable;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using project.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace project.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TeachersController : Controller
    {
        private readonly DataContext _context;
        public TeachersController(DataContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var mnList = _context.Teachers.OrderBy(m => m.TeacherID).ToList();
            return View(mnList);
        }

        //xóa
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();
            var mn = _context.Teachers.Find(id);
            if (mn == null)
                return NotFound();
            return View(mn);
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var delTeachers = _context.Teachers.Find(id);
            if (delTeachers == null)
                return NotFound();
            _context.Teachers.Remove(delTeachers);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        //Thêm
        public IActionResult Create()
        {
            var mnList = _context.Teachers
                        .Select(m => new SelectListItem
                        {
                            Text = m.FullName,
                            Value = m.TeacherID.ToString()
                        }).ToList();
            mnList.Insert(0, new SelectListItem
            {
                Text = "--- Chọn giáo viên ---",
                Value = "0"
            });
            ViewBag.TeacherList = mnList;
            return View();
        }
        [HttpPost]
        public IActionResult Create(tblTeachers teacher)
        {
            if (ModelState.IsValid)
            {
                _context.Teachers.Add(teacher);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(teacher);
        }
        //Sửa
        public IActionResult Edit(int id)
        {
            var teacher = _context.Teachers.FirstOrDefault(t => t.TeacherID == id);
            if (teacher == null)
            {
                return NotFound();
            }
            return View(teacher);
        }

        // POST: Admin/Teachers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(tblTeachers teacher)
        {
            if (ModelState.IsValid)
            {
                _context.Update(teacher);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(teacher);
        }

    }
}
