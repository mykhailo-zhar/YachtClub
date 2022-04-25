using Microsoft.AspNetCore.Mvc;
using Project.Database;
using Project.Migrations;
using Project.Models;
using System.Linq;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Project.Controllers
{
    public class StaffController : Controller
    {
        public DataContext Context;
        public StaffController(DataContext context)
        {
            Context = context;
        }

        #region Person
        public IActionResult Staff()
        {
            var Person = Context.Person.FromSqlRaw(@"select * from Staff").OrderBy(p => p.Id);
            return View(Person);
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
                .OrderBy(p => p.Id)
                ;
            return View(StaffPosition);
        }

        public IActionResult DetailsStaffPosition(string id)
        {
            var StaffPosition = Context.StaffPosition.First(p => p.Id == int.Parse(id));
            return View("_Details", new DetailsViewModel
            {
                Description = StaffPosition.Description,
                ButtonsViewModel = new EditorBottomButtonsViewModel
                {
                    BackAction = typeof(StaffPosition).Name
                }
            });
        }


        public IActionResult CreateStaffPosition()
        {
            var StaffPosition = new StaffPosition();
            ViewData["Staff"] = Context.Person.ToList();
            ViewData["Position"] = Context.Position.ToList();
            return View("StaffPositionEditor", ObjectViewModelFactory<StaffPosition>.Create(StaffPosition));
        }

        [HttpPost]
        public async Task<IActionResult> CreateStaffPosition([FromForm] ObjectViewModel<StaffPosition> StaffPosition)
        {
            if (ModelState.IsValid)
            {
                StaffPosition.Object.Startdate = DateTime.Now;
                Context.StaffPosition.Add(StaffPosition.Object);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(StaffPosition));
            }

            return View("StaffPositionEditor", ObjectViewModelFactory<StaffPosition>.Create(StaffPosition.Object));
        }

        public IActionResult EditStaffPosition(string id)
        {
            var StaffPosition = Context.StaffPosition.First(p => p.Id == int.Parse(id));
            ViewData["Staff"] = Context.Person.ToList();
            ViewData["Position"] = Context.Position.ToList();
            return View("StaffPositionEditor", ObjectViewModelFactory<StaffPosition>.Edit(StaffPosition));
        }

        [HttpPost]
        public async Task<IActionResult> EditStaffPosition([FromForm] ObjectViewModel<StaffPosition> StaffPosition)
        {
            if (ModelState.IsValid)
            {
                if (StaffPosition.Option[0]) StaffPosition.Object.Enddate = DateTime.Now;
                Context.StaffPosition.Update(StaffPosition.Object);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(StaffPosition));
            }
            return View("StaffPositionEditor", ObjectViewModelFactory<StaffPosition>.Edit(StaffPosition.Object));
        }


        public IActionResult DeleteStaffPosition(string id)
        {
            var StaffPosition = Context.StaffPosition.First(p => p.Id == int.Parse(id));
            ViewData["Staff"] = null;
            ViewData["Position"] = null;
            return View("StaffPositionEditor", ObjectViewModelFactory<StaffPosition>.Delete(StaffPosition));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteStaffPosition([FromForm] ObjectViewModel<StaffPosition> StaffPosition)
        {

            Context.StaffPosition.Remove(StaffPosition.Object);
            await Context.SaveChangesAsync();
            return RedirectToAction(nameof(StaffPosition));

        }
        #endregion
    }
}
