using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using project.Areas.Admin.Models; // 👈 Thêm dòng này nếu chưa có
using project.Models;
namespace project.Areas.Admin.Models
{
    [Table("tblFields")]
    public class tblFields
    {
        [Key]
        public int FieldID { get; set; } // 👈 Sử dụng long cho khóa chính

        [Required]
        [StringLength(100)]
        public string FieldName { get; set; } = string.Empty; // 👈 tránh lỗi non-null

        [StringLength(255)]
        public string? Description { get; set; }

        public long? TeacherID { get; set; }

        [ForeignKey("TeacherID")]
        public tblTeachers? Teacher { get; set; } // 👈 Quan hệ với bảng tblTeachers

        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        [StringLength(50)]
        public string? Status { get; set; } = "Đang hoạt động";
    }
}
