using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using project.Models;
using System;
using System.Threading.Tasks;

namespace project.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class EvaluationsController : Controller
    {
        private readonly DataContext _context;

        public EvaluationsController(DataContext context)
        {
            _context = context;
        }

        // ==========================================================
        // === HÀM CREATE (GET) ĐÃ "ĐỘ" LẠI LOGIC REDIRECT ===
        // ==========================================================
        [HttpGet]
        public async Task<IActionResult> Create(int projectId)
        {
            var project = await _context.Projects.FindAsync(projectId);
            if (project == null) return NotFound();

            if (project.Status != "Hoàn thành")
            {
                TempData["Error"] = "Lỗi: Bạn chỉ có thể đánh giá dự án đã 'Hoàn thành'.";
                return RedirectToAction("Index", "Projects");
            }
            
            // KIỂM TRA XEM ĐÃ CÓ ĐÁNH GIÁ CHƯA
            var existingEvaluation = await _context.Evaluations.FirstOrDefaultAsync(e => e.ProjectID == projectId);
            if (existingEvaluation != null)
            {
                // NẾU ĐÃ CÓ: "LÁI" (REDIRECT) SANG TRANG EDIT
                return RedirectToAction("Edit", new { id = existingEvaluation.EvaluationID });
            }

            // NẾU CHƯA CÓ: MỞ TRANG CREATE BÌNH THƯỜNG
            await PopulateTeachersDropdown(); // Load TẤT CẢ giáo viên
            var model = new tblEvaluations
            {
                ProjectID = projectId,
                Project = project,
                EvaluationDate = DateTime.Now
            };

            return View(model);
        }

        // POST: Admin/Evaluations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(tblEvaluations evaluation)
        {
            evaluation.EvaluationDate = DateTime.Now;
            
            if (ModelState.IsValid)
            {
                _context.Evaluations.Add(evaluation);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Projects");
            }

            evaluation.Project = await _context.Projects.FindAsync(evaluation.ProjectID);
            await PopulateTeachersDropdown(evaluation.TeacherID);
            return View(evaluation);
        }

        // ==========================================================
        // === TASK MỚI: SỬA ĐÁNH GIÁ (GET) ===
        // ==========================================================
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var evaluation = await _context.Evaluations
                .Include(e => e.Project) // Lấy thông tin Project
                .FirstOrDefaultAsync(e => e.EvaluationID == id);

            if (evaluation == null) return NotFound();

            // Load danh sách GV và chọn sẵn GV đã đánh giá
            await PopulateTeachersDropdown(evaluation.TeacherID);
            return View(evaluation);
        }

        // ==========================================================
        // === TASK MỚI: SỬA ĐÁNH GIÁ (POST) ===
        // ==========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, tblEvaluations evaluation)
        {
            if (id != evaluation.EvaluationID) return NotFound();
            
            evaluation.EvaluationDate = DateTime.Now; // Cập nhật ngày sửa

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(evaluation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Evaluations.Any(e => e.EvaluationID == evaluation.EvaluationID))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction("Index", "Projects");
            }
            
            // Nếu lỗi, load lại
            evaluation.Project = await _context.Projects.FindAsync(evaluation.ProjectID);
            await PopulateTeachersDropdown(evaluation.TeacherID);
            return View(evaluation);
        }

        // --- HÀM HELPER (Tải danh sách TẤT CẢ giáo viên) ---
        private async Task PopulateTeachersDropdown(object? selectedTeacher = null)
        {
            var allTeachers = await _context.Teachers.OrderBy(t => t.FullName).ToListAsync();
            ViewBag.TeacherList = new SelectList(allTeachers, "TeacherID", "FullName", selectedTeacher);
        }
    }
}
