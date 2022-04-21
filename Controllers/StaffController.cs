using Microsoft.AspNetCore.Mvc;
using Project.Database;
using Project.Migrations;
using Project.Models;
using System.Linq;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

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
            var Staff = Context.Staff.OrderBy(p => p.Id);
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

            return View("StaffEditor", ObjectViewModelFactory<Staff>.Edit(staff.Object));
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

        #region Position
        public IActionResult Position()
        {
            var Position = Context.Position.OrderBy(p => p.Id);
            return View(Position);
        }

        public IActionResult CreatePosition()
        {
            var Position = new Position();
            return View("PositionEditor", ObjectViewModelFactory<Position>.Create(Position));
        }

        [HttpPost]
        public async Task<IActionResult> CreatePosition([FromForm] ObjectViewModel<Position> Position)
        {
            if (ModelState.IsValid)
            {
                Context.Position.Add(Position.Object);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(Position));
            }

            return View("PositionEditor", ObjectViewModelFactory<Position>.Create(Position.Object));
        }

        public IActionResult EditPosition(string id)
        {
            var Position = Context.Position.First(p => p.Id == int.Parse(id));
            return View("PositionEditor", ObjectViewModelFactory<Position>.Edit(Position));
        }

        [HttpPost]
        public async Task<IActionResult> EditPosition([FromForm] ObjectViewModel<Position> Position)
        {
            if (ModelState.IsValid)
            {
                Context.Position.Update(Position.Object);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(Position));
            }

            return View("PositionEditor", ObjectViewModelFactory<Position>.Edit(Position.Object));
        }


        public IActionResult DeletePosition(string id)
        {
            var Position = Context.Position.First(p => p.Id == int.Parse(id));
            return View("PositionEditor", ObjectViewModelFactory<Position>.Delete(Position));
        }

        [HttpPost]
        public async Task<IActionResult> DeletePosition([FromForm] ObjectViewModel<Position> Position)
        {

            Context.Position.Remove(Position.Object);
            await Context.SaveChangesAsync();
            return RedirectToAction(nameof(Position));

        }
        #endregion

        #region StaffPosition
        public IActionResult StaffPosition()
        {
            var StaffPosition = Context.StaffPosition
                .Include(p => p.Position)
                .Include(p => p.Staff)
                ;
            return View(StaffPosition);
        }

        public IActionResult CreateStaffPosition()
        {
            var StaffPosition = new StaffPosition();
            var Staff = Context.Staff.ToList();
            var Position = Context.Position.ToList();
            return View("StaffPositionEditor", TwineViewModelFactory<StaffPosition,Staff, Position>.Create(StaffPosition, Staff, Position));
        }

        [HttpPost]
        public async Task<IActionResult> CreateStaffPosition([FromForm] TwineViewModel<StaffPosition,Staff, Position> StaffPosition)
        {
            
            if (ModelState.IsValid)
            {
                Context.StaffPosition.Add(StaffPosition.baseObject.Object);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(StaffPosition));
            }

            return View("StaffPositionEditor", TwineViewModelFactory<StaffPosition,Staff, Position>.Create(StaffPosition.baseObject.Object, null, null));
        }

        public IActionResult EditStaffPosition(string id)
        {
            var StaffPosition = Context.StaffPosition.First(p => p.Id == int.Parse(id));
            var Staff = Context.Staff.ToList();
            var Position = Context.Position.ToList();
            return View("StaffPositionEditor", TwineViewModelFactory<StaffPosition,Staff, Position>.Edit(StaffPosition, Staff, Position));
        }

        [HttpPost]
        public async Task<IActionResult> EditStaffPosition([FromForm] TwineViewModel<StaffPosition,Staff, Position> StaffPosition)
        {
            if (ModelState.IsValid)
            {
                Context.StaffPosition.Update(StaffPosition.baseObject.Object);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(StaffPosition));
            }

            return View("StaffPositionEditor", TwineViewModelFactory<StaffPosition,Staff, Position>.Edit(StaffPosition.baseObject.Object, null, null));
        }


        public IActionResult DeleteStaffPosition(string id)
        {
            var StaffPosition = Context.StaffPosition.First(p => p.Id == int.Parse(id));
            var Staff = Context.Staff.ToList();
            var Position = Context.Position.ToList();
            return View("StaffPositionEditor", TwineViewModelFactory<StaffPosition,Staff, Position>.Delete(StaffPosition, Staff, Position));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteStaffPosition([FromForm] TwineViewModel<StaffPosition,Staff, Position> StaffPosition)
        {

            Context.StaffPosition.Remove(StaffPosition.baseObject.Object);
            await Context.SaveChangesAsync();
            return RedirectToAction(nameof(StaffPosition));
        }
        #endregion
    }
}
