using Microsoft.AspNetCore.Mvc;
using Project.Database;
using Project.Migrations;
using Project.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Controllers
{
    public class StaffController : Controller
    {
        public DataContext Context;
        public StaffController(DataContext context)
        {
            Context = context;
        }
        public IActionResult Staff()
        {
            var Staff = Context.Staff;
            return View(Staff);
        }

        public IActionResult EditStaff(string id)
        {
            var Staff = Context.Staff.FirstOrDefault(p => p.Id == int.Parse(id));
            return View("StaffEditor", ObjectViewModelFactory<Staff>.Edit(Staff));
        }

        [HttpPost]
        public async Task<IActionResult> EditStaff([FromForm] Staff staff)
        {
            if (ModelState.IsValid)
            {
                Context.Staff.Update(staff);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(Staff));
            }

            return View("StaffEditor", ObjectViewModelFactory<Staff>.Edit(staff));
        }
    }
}
