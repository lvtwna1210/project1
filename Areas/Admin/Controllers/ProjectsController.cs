using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using project.Models;
using System.Linq;
using System.Threading.Tasks; 
using System.Collections.Generic;
using project.Areas.Admin.Models; 
using System; 

namespace project.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProjectsController : Controller
    {
        private readonly DataContext _context;

        public ProjectsController(DataContext context)
        {
            _context = context;
        }

        // GET: Admin/Projects (Trang Index)
        public async Task<IActionResult> Index()
        {
            var projects = await _context.Projects
                .Include(p => p.ProjectFields).ThenInclude(pf => pf.Field)
                .Include(p => p.ProjectTeachers).ThenInclude(pt => pt.Teacher)
                .Include(p => p.ProjectStudents).ThenInclude(ps => ps.Student)
                .Include(p => p.ProjectTasks) 
                .OrderByDescending(p => p.ProjectID)
                .ToListAsync();
            return View(projects);
        }

        // GET: Admin/Projects/Create (Trang Tạo)
        public async Task<IActionResult> Create()
        {
            await PopulateViewBagData(null);
            return View(new tblProject()); 
        }

        // POST: Admin/Projects/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(tblProject project, List<int> fieldIds, List<int> teacherIds, List<int> studentIds)
        {
            if (ModelState.IsValid)
            {
                UpdateProjectRelations(project, fieldIds, teacherIds, studentIds); 
                _context.Projects.Add(project);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            await PopulateViewBagData(project);
            return View(project);
        }

        // GET: Admin/Projects/Edit/5 (Trang Sửa)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var project = await _context.Projects
                .Include(p => p.ProjectFields) 
                .Include(p => p.ProjectTeachers) 
                .Include(p => p.ProjectStudents) 
                .FirstOrDefaultAsync(p => p.ProjectID == id);
            if (project == null) return NotFound();
            await PopulateViewBagData(project); 
            return View(project);
        }

        // POST: Admin/Projects/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, tblProject project, List<int> fieldIds, List<int> teacherIds, List<int> studentIds)
        {
            if (id != project.ProjectID) return NotFound();

            var projectToUpdate = await _context.Projects
                .Include(p => p.ProjectFields) 
                .Include(p => p.ProjectTeachers) 
                .Include(p => p.ProjectStudents) 
                .FirstOrDefaultAsync(p => p.ProjectID == id);
            if (projectToUpdate == null) return NotFound();
            
            if (ModelState.IsValid)
            {
                projectToUpdate.ProjectName = project.ProjectName;
                projectToUpdate.Description = project.Description; 
                projectToUpdate.StartDate = project.StartDate;
                projectToUpdate.EndDate = project.EndDate; 
                projectToUpdate.Status = project.Status; 
                UpdateProjectRelations(projectToUpdate, fieldIds, teacherIds, studentIds); 
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException) { /* ... (xử lý lỗi) ... */ }
                return RedirectToAction(nameof(Index));
            }
            await PopulateViewBagData(project);
            return View(project);
        }

        // GET: Admin/Projects/Delete/5 (Trang Xóa)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var project = await _context.Projects
                .Include(p => p.ProjectFields).ThenInclude(pf => pf.Field)
                .Include(p => p.ProjectTeachers).ThenInclude(pt => pt.Teacher)
                .Include(p => p.ProjectStudents).ThenInclude(ps => ps.Student) 
                .FirstOrDefaultAsync(m => m.ProjectID == id);
            if (project == null) return NotFound();
            return View(project);
        }

        // ==========================================================
        // === HÀM DELETE (POST) ĐÃ SỬA LỖI FK_TaskAssignments_Tasks ===
        // ==========================================================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await _context.Projects
                .Include(p => p.ProjectFields)
                .Include(p => p.ProjectTeachers)
                .Include(p => p.ProjectStudents)
                .Include(p => p.ProjectTasks) // <-- Lấy Tasks (con)
                    .ThenInclude(t => t.Assignments) // <-- (SỬA) LẤY ASSIGNMENTS (CHÁU)
                .Include(p => p.Evaluations) 
                .FirstOrDefaultAsync(p => p.ProjectID == id);
                
            if (project != null)
            {
                // 1. Xóa các bảng N-N (Project-level)
                _context.ProjectFields.RemoveRange(project.ProjectFields);
                _context.ProjectTeachers.RemoveRange(project.ProjectTeachers);
                _context.ProjectStudents.RemoveRange(project.ProjectStudents);

                // 2. Xóa các bảng con (1-N)
                _context.Evaluations.RemoveRange(project.Evaluations);

                // 3. (SỬA) Xóa các "cháu" (TaskAssignments) TRƯỚC
                foreach (var task in project.ProjectTasks)
                {
                    _context.TaskAssignments.RemoveRange(task.Assignments);
                }

                // 4. Xóa các "con" (Tasks) - (Bây giờ đã an toàn)
                _context.Tasks.RemoveRange(project.ProjectTasks); 

                // 5. Xóa chính nó (Project)
                _context.Projects.Remove(project);
                
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // ----- HÀM AJAX VÀ HÀM HELPER -----
        
        [HttpGet]
        public async Task<JsonResult> GetProjectDetails(int projectId)
        {
            var project = await _context.Projects
                .Include(p => p.ProjectFields).ThenInclude(pf => pf.Field)
                .Include(p => p.ProjectTeachers).ThenInclude(pt => pt.Teacher)
                .Include(p => p.ProjectStudents).ThenInclude(ps => ps.Student)
                .FirstOrDefaultAsync(p => p.ProjectID == projectId);

            if (project == null) return Json(new { error = "Không tìm thấy dự án." });

            var tasks = await _context.Tasks
                .Where(t => t.ProjectID == projectId)
                .Include(t => t.Assignments).ThenInclude(a => a.Student) 
                .Include(t => t.Assignments).ThenInclude(a => a.Teacher) 
                .OrderBy(t => t.StartDate)
                .Select(t => new {
                    taskName = t.TaskName,
                    status = t.Status,
                    priority = t.Priority,
                    description = t.Description, 
                    attachment = t.Attachment, 
                    endDate = t.EndDate.HasValue ? t.EndDate.Value.ToString("dd/MM/yyyy") : "--",
                    assignments = t.Assignments.Select(a => new {
                        name = a.Teacher != null ? a.Teacher.FullName : a.Student.FullName,
                        type = a.Teacher != null ? "Giáo viên" : "Học sinh",
                        role = a.Role
                    }).ToList()
                })
                .ToListAsync();

            var teachers = project.ProjectTeachers.Select(pt => pt.Teacher?.FullName).Where(t => t != null).ToList();
            var students = project.ProjectStudents.Select(ps => ps.Student?.FullName).Where(s => s != null).ToList();
            var fields = project.ProjectFields.Select(pf => pf.Field?.FieldName).Where(f => f != null).ToList();

            var evaluation = await _context.Evaluations
                .Include(e => e.Teacher) 
                .FirstOrDefaultAsync(e => e.ProjectID == projectId);

            return Json(new {
                projectName = project.ProjectName,
                description = project.Description,
                status = project.Status,
                startDate = project.StartDate.HasValue ? project.StartDate.Value.ToString("dd/MM/yyyy") : "--",
                endDate = project.EndDate.HasValue ? project.EndDate.Value.ToString("dd/MM/yyyy") : "--",
                tasks = tasks,         
                teachers = teachers,     
                students = students,     
                fields = fields,
                evaluation = evaluation == null ? null : new {
                    score = evaluation.Score,
                    comments = evaluation.Comments,
                    teacherName = evaluation.Teacher?.FullName ?? "Chưa rõ", 
                    date = evaluation.EvaluationDate.ToString("dd/MM/yyyy")
                }
            });
        }
        
        [HttpGet] 
        public JsonResult GetTeachersForFields([FromQuery] List<int> fieldIds)
        {
            var allTeachers = _context.Teachers.Select(t => new { t.TeacherID, t.FullName }).ToList();
            if (fieldIds == null || !fieldIds.Any())
            {
                return Json(allTeachers);
            }
            
            var teacherIds = _context.tblTeacherFields 
                .Where(tf => fieldIds.Contains(tf.FieldID)) 
                .Select(tf => tf.TeacherID)
                .Distinct()
                .ToList();

            var filteredTeachers = allTeachers.Where(t => teacherIds.Contains(t.TeacherID)).ToList();
            return Json(filteredTeachers);
        }

        private async Task PopulateViewBagData(tblProject? project = null) 
        {
            ViewBag.StatusList = new SelectList(new List<string> { "Mới", "Đang thực hiện", "Hoàn thành", "Bị hủy" });
            ViewBag.AllFields = await _context.Fields.OrderBy(f => f.FieldName).ToListAsync();
            ViewBag.AllTeachers = await _context.Teachers.OrderBy(t => t.FullName).ToListAsync();
            ViewBag.AllStudents = await _context.Students.OrderBy(s => s.FullName).ToListAsync();
            
            if (project != null && project.ProjectID > 0)
            {
                ViewBag.SelectedFieldIds = project.ProjectFields!.Select(pf => pf.FieldID).ToList();
                ViewBag.SelectedTeacherIds = project.ProjectTeachers!.Select(pt => pt.TeacherID).ToList();
                ViewBag.SelectedStudentIds = project.ProjectStudents!.Select(ps => ps.StudentID).ToList();
            }
            else
            {
                ViewBag.SelectedFieldIds = new List<int>();
                ViewBag.SelectedTeacherIds = new List<int>();
                ViewBag.SelectedStudentIds = new List<int>();
            }
        }

        private void UpdateProjectRelations(tblProject projectToUpdate, 
            IEnumerable<int> selectedFieldIDs, 
            IEnumerable<int> selectedTeacherIDs, 
            IEnumerable<int> selectedStudentIDs)
        {
            projectToUpdate.ProjectFields ??= new List<tblProjectFields>();
            projectToUpdate.ProjectTeachers ??= new List<tblProjectTeachers>();
            projectToUpdate.ProjectStudents ??= new List<tblProjectStudents>();

            projectToUpdate.ProjectFields.Clear();
            if (selectedFieldIDs != null)
            {
                foreach (var fieldID in selectedFieldIDs)
                {
                    projectToUpdate.ProjectFields.Add(new tblProjectFields { ProjectID = projectToUpdate.ProjectID, FieldID = fieldID });
                }
            }

            projectToUpdate.ProjectTeachers.Clear();
            if (selectedTeacherIDs != null)
            {
                foreach (var teacherID in selectedTeacherIDs)
                {
                    projectToUpdate.ProjectTeachers.Add(new tblProjectTeachers { ProjectID = projectToUpdate.ProjectID, TeacherID = teacherID });
                }
            }

            projectToUpdate.ProjectStudents.Clear();
            if (selectedStudentIDs != null)
            {
                foreach (var studentID in selectedStudentIDs)
                {
                    projectToUpdate.ProjectStudents.Add(new tblProjectStudents { ProjectID = projectToUpdate.ProjectID, StudentID = studentID });
                }
            }
        }
    }
}
