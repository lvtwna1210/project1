using project.Models;
using Microsoft.AspNetCore.Mvc;

namespace project.Components
{
    [ViewComponent(Name = "TeachersView")]
    public class TeachersViewComponent : ViewComponent
    {
        private readonly DataContext _context;

        public TeachersViewComponent(DataContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var mnList = await Task.FromResult(_context.Teachers.ToList());
            return View(mnList);
        }
    }
}
