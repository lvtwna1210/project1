using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using project.Models;
namespace project.Models
{
    [Table("tblTeachers")]
    public class tblTeachers
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TeacherID { get; set; }


         [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
        public string? FullName { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn giới tính")]
        public string? Gender { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string? Email { get; set; }

        public string? Address { get; set; }

        public string? Position { get; set; }
        public string? Subject { get; set; }
        public string? Qualification { get; set; }

        [DataType(DataType.Date)]
        public DateTime? HireDate { get; set; }

        public string? Status { get; set; }
    }
}
