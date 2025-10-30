using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using project.Models;
namespace project.Models
{
    [Table("tblStudents")]
    public class tblStudents
    {
        [Key]
        public int StudentID { get; set; }

        [Required(ErrorMessage = "Họ và tên không được để trống")]
        public string? FullName { get; set; }

        public string? Gender { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Ngày sinh")]
        public DateTime? DateOfBirth { get; set; }

        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }

        [Display(Name = "Lớp học")]
        public string? Class { get; set; }

        [Display(Name = "Khối")]
        public string? Grade { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Ngày nhập học")]
        public DateTime? EnrollmentDate { get; set; }

        public string? Status { get; set; }
    }
}
