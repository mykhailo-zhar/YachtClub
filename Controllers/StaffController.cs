using Microsoft.AspNetCore.Mvc;
using Project.Database;
using Project.Migrations;
using Project.Models;
using System.Linq;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace Project.Controllers
{
    [Authorize(Policy = "Staff")]
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
            var Person = Context.Person.FromSqlRaw(@"select * from Staff")
                .Select(p => new PortfolioViewModel
                {
                    Person = p,
                    Portfolio = Context.StaffPosition
                    .Where(z => z.Staffid == p.Id)
                    .Include(l => l.Position)
                    .ToList(),
                    YachtPortfolio = Context.YachtCrew
                    .Include(p => p.Yacht)
                        .ThenInclude(p => p.Type)
                    .Include(p => p.Crew)
                        .ThenInclude(p => p.Staff)
                    .Include(p => p.Crew)
                        .ThenInclude(p => p.Position)
                    .Where(l => l.Crew.Staffid == p.Id)
                    .Where(p => p.Crew.Position.Crewposition)
                    .OrderByDescending(p => p.Enddate ?? DateTime.Now)
                    .ToList()
                })
                .OrderBy(p => p.Person.Id);
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
                try
                {
                    Context.Position.Add(Position.Object);
                    await Context.SaveChangesAsync();
                    return RedirectToAction(nameof(Position));
                }
                catch (Exception exception)
                {
                    this.HandleException(exception);
                }
                if (ModelState.IsValid)
                {
                    return RedirectToAction(nameof(Position));
                }
                return View("PositionEditor", ObjectViewModelFactory<Position>.Create(Position.Object));
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
                try
                {
                    Context.Position.Update(Position.Object);
                    await Context.SaveChangesAsync();
                    return RedirectToAction(nameof(Position));
                }
                catch (Exception exception)
                {
                    this.HandleException(exception);
                }
                if (ModelState.IsValid)
                {
                    return RedirectToAction(nameof(Position));
                }
                return View("PositionEditor", ObjectViewModelFactory<Position>.Edit(Position.Object));
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


            try
            {
                Context.Position.Remove(Position.Object);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(Position));
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
            }
            if (ModelState.IsValid)
            {
                return RedirectToAction(nameof(Position));
            }
            return View("PositionEditor", ObjectViewModelFactory<Position>.Delete(Position.Object));

        }
        #endregion

        #region StaffPosition

        private void ConfigureViewBagStaffPosition()
        {
            ViewData["Staff"] = Context.Person.ToList();
            ViewData["Position"] = Context.Position.ToList();
        }
        private IActionResult PrivateCreateStaffPosition(int sid = 0, StaffPosition StaffPosition = null)
        {
            StaffPosition = StaffPosition ?? new StaffPosition { Staffid = sid };
            ConfigureViewBagStaffPosition();
            return View("StaffPositionEditor", ObjectViewModelFactory<StaffPosition>.Create(StaffPosition));
        }
        public IActionResult CreateStaffPosition(int sid) => PrivateCreateStaffPosition(sid);

        [HttpPost]
        public async Task<IActionResult> CreateStaffPosition([FromForm] ObjectViewModel<StaffPosition> StaffPosition)
        {
            if (ModelState.IsValid)
            {
                using (var trans = Context.Database.BeginTransaction())
                {
                    try
                    {
                        StaffPosition.Object.Startdate = DateTime.Now;
                        Context.StaffPosition.Add(StaffPosition.Object);
                        await Context.SaveChangesAsync();
                        trans.Commit();
                        return RedirectToAction(nameof(Staff));
                    }
                    catch (Exception exception)
                    {
                        trans.Rollback();
                        this.HandleException(exception);
                    }
                }
            }

            return PrivateCreateStaffPosition(StaffPosition: StaffPosition.Object);
        }

        public IActionResult EditStaffPosition(string id)
        {
            var StaffPosition = Context.StaffPosition.First(p => p.Id == int.Parse(id));
            ViewData["Position"] = Context.Position.ToList();
            return View("StaffPositionEditor", ObjectViewModelFactory<StaffPosition>.Edit(StaffPosition));
        }

        [HttpPost]
        public async Task<IActionResult> EditStaffPosition([FromForm] ObjectViewModel<StaffPosition> StaffPosition)
        {
            if (ModelState.IsValid)
            {

                try
                {
                    if (StaffPosition.Option[0]) StaffPosition.Object.Enddate = DateTime.Now;
                    Context.StaffPosition.Update(StaffPosition.Object);
                    await Context.SaveChangesAsync();
                    return RedirectToAction(nameof(Staff));
                }
                catch (Exception exception)
                {
                    this.HandleException(exception);
                }
            }
            return View("StaffPositionEditor", ObjectViewModelFactory<StaffPosition>.Edit(StaffPosition.Object));
        }


        public IActionResult DeleteStaffPosition(string id)
        {
            var StaffPosition = Context.StaffPosition.First(p => p.Id == int.Parse(id));
            ViewData["Position"] = Context.Position.ToList();
            return View("StaffPositionEditor", ObjectViewModelFactory<StaffPosition>.Delete(StaffPosition));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteStaffPosition([FromForm] ObjectViewModel<StaffPosition> StaffPosition)
        {
            try
            {
                Context.StaffPosition.Remove(StaffPosition.Object);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(Staff));
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
            }
            return View("StaffPositionEditor", ObjectViewModelFactory<StaffPosition>.Delete(StaffPosition.Object));

        }
        #endregion
    }
}
