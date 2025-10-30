using Microsoft.EntityFrameworkCore;
using project.Areas.Admin.Models;

namespace project.Models
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        // DbSet cho bảng giáo viên
        public DbSet<tblTeachers> Teachers { get; set; }

        // DbSet cho bảng học sinh
        public DbSet<tblStudents> Students { get; set; }

        // DbSet cho bảng lĩnh vực
        public DbSet<tblFields> tblFields { get; set; }

        // DbSet cho menu admin
        public DbSet<AdminMenu> AdminMenus { get; set; }
    }
}
