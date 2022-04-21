using Microsoft.AspNetCore.Mvc;
using Project.Database;
using Project.Migrations;
using Project.Models;
using System.Linq;
using System;
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

        #region Staff
        public IActionResult Staff()
        {
            var Staff = Context.Staff;
            return View(Staff);
        }

        public IActionResult EditStaff(string id)
        {
            var Staff = Context.Staff.First(p => p.Id == int.Parse(id));
            return View("StaffEditor", ObjectViewModelFactory<Staff>.Edit(Staff));
        }

        [HttpPost]
        public async Task<IActionResult> EditStaff([FromForm] ObjectViewModel<Staff> staff)
        {
            if (ModelState.IsValid)
            {
                Context.Staff.Update(staff.Object);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(Staff));
            }

            return View("StaffEditor", ObjectViewModelFactory<Staff>.Edit(staff));
        }

        public IActionResult CreateStaff()
        {
            var Staff = new Staff();
            return View("StaffEditor", ObjectViewModelFactory<Staff>.Create(Staff));
        }

        [HttpPost]
        public async Task<IActionResult> CreateStaff([FromForm] ObjectViewModel<Staff> staff)
        {
            if (ModelState.IsValid)
            {
                staff.Object.Hiringdate = DateTime.Now;
                Context.Staff.Add(staff.Object);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(Staff));
            }

            return View("StaffEditor", ObjectViewModelFactory<Staff>.Create(staff.Object));
        }
        public IActionResult DeleteStaff(string id)
        {
            var Staff = Context.Staff.First(p => p.Id == int.Parse(id));
            return View("StaffEditor", ObjectViewModelFactory<Staff>.Delete(Staff));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteStaff([FromForm] ObjectViewModel<Staff> staff)
        {

            Context.Staff.Remove(staff.Object);
            await Context.SaveChangesAsync();
            return RedirectToAction(nameof(Staff));

        }
        #endregion
    }
}
