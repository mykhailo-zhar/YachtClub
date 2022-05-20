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
    //TODO: Организовать доступ к персоналу клиентов
    //TODO: Описание A) Обезопасить от NULL в базе / здесь на сервере
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

        //TODO: Staff_Position: Организовать персонал с клиентом
        ///TODO: Hired_Staff: Триггер на 1 к 1 ИЛИ Организовать связь 1 к 0, второе предпочтительней. 
        ///Или всё таки допустить, что у одного сотрудника(члена-экипажа) может быть много клиентов, но это не так.

        #region StaffPosition
        public IActionResult CreateStaffPosition(int sid)
        {
            var StaffPosition = new StaffPosition { Staffid = sid};
            ViewData["Staff"] = Context.Person.ToList();
            ViewData["Position"] = Context.Position.ToList();
            return View("StaffPositionEditor", ObjectViewModelFactory<StaffPosition>.Create(StaffPosition));
        }

        [HttpPost]
        public async Task<IActionResult> CreateStaffPosition([FromForm] ObjectViewModel<StaffPosition> StaffPosition)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    StaffPosition.Object.Startdate = DateTime.Now;
                    Context.StaffPosition.Add(StaffPosition.Object);
                    await Context.SaveChangesAsync();
                    return RedirectToAction(nameof(Staff));
                }
                catch (Exception exception)
                {
                    this.HandleException(exception);
                }
                if (ModelState.IsValid)
                {
                    return RedirectToAction(nameof(Staff));
                }
                return View("StaffPositionEditor", ObjectViewModelFactory<StaffPosition>.Create(StaffPosition.Object));
            }

            return View("StaffPositionEditor", ObjectViewModelFactory<StaffPosition>.Create(StaffPosition.Object));
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
                if (ModelState.IsValid)
                {
                    return RedirectToAction(nameof(Staff));
                }
                return View("StaffPositionEditor", ObjectViewModelFactory<StaffPosition>.Edit(StaffPosition.Object));
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
            if (ModelState.IsValid)
            {
                return RedirectToAction(nameof(Staff));
            }
            return View("StaffPositionEditor", ObjectViewModelFactory<StaffPosition>.Delete(StaffPosition.Object));

        }
        #endregion

        //TODO: Капитаны: Запихнуть в портфолио персонала...
        //TODO: Капитаны: Триггер на существование контракта...
        #region YachtCrew
        public IActionResult YachtCrew()
        {
            var Object = Context.YachtCrew
                .Include(p => p.Yacht)
                    .ThenInclude(p => p.Type)
                .Include(p => p.Crew)
                    .ThenInclude(p => p.Position)
                .Include(p => p.Crew)
                    .ThenInclude(p => p.Staff)
                   
                /*Включение навигационных свойств*/
                .OrderBy(p => p.Yachtid).ThenBy(p => p.Startdate).ThenBy(p => p.Crew.Positionid);
            return View(Object);
        }

        private void YachtCrewConfigureViewBag()
        {
            ViewBag.Crew = Context.StaffPosition
                .Include(p => p.Position)
                .Include(p => p.Staff)
                .Where(p => p.Position.Crewposition)
                .Where(p => p.Enddate == null)
                .Where(p => !Context.YachtCrew
                              .Where(y => y.Enddate == null)
                              .Any(o => o.Crewid == p.Id))
                ;
            ViewBag.Yacht = Context.Yacht
                .Include(p => p.Type)
                .Where(p => Context.Busyyacht.Any(b => b.Id == p.Id && b.Val.Value))
                .Where(p => p.Type.Crewcapacity - Context.YachtCrew.Where(c => c.Enddate == null && c.Yachtid == p.Id).Count() > 0)
                ;
        }

        private IActionResult LocalEditYachtCrew(string id, YachtCrew YachtCrew = null)
        {
            YachtCrew = YachtCrew ?? Context.YachtCrew
               /*Включение навигационных свойств*/
               .First(p => p.Id == int.Parse(id));
            /*Включение навигационных свойств*/
            var Model = ObjectViewModelFactory<YachtCrew>.Edit(YachtCrew);
            YachtCrewConfigureViewBag();
            return View("YachtCrewEditor", Model);
        }

        public IActionResult EditYachtCrew(string id) => LocalEditYachtCrew(id);

        [HttpPost]
        public async Task<IActionResult> EditYachtCrew([FromForm] ObjectViewModel<YachtCrew> YachtCrew)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (YachtCrew.Option[0])
                    {
                        YachtCrew.Object.Enddate = DateTime.Now;
                    }
                    YachtCrew.Object.Description = Methods.CoalesceString(YachtCrew.Object.Description);
                    Context.YachtCrew.Update(YachtCrew.Object);
                    await Context.SaveChangesAsync();
                    return RedirectToAction(nameof(YachtCrew));
                }
                catch (Exception exception)
                {
                    this.HandleException(exception);
                }
            }

            return LocalEditYachtCrew($"{YachtCrew.Object.Id}", YachtCrew.Object);
        }

        private IActionResult LocalCreateYachtCrew(YachtCrew YachtCrew = null)
        {
            YachtCrew = YachtCrew ?? new YachtCrew();
            /*Включение навигационных свойств*/
            var Model = ObjectViewModelFactory<YachtCrew>.Create(YachtCrew);
            YachtCrewConfigureViewBag();
            return View("YachtCrewEditor", Model);
        }

        public IActionResult CreateYachtCrew() => LocalCreateYachtCrew(null);

        [HttpPost]
        public async Task<IActionResult> CreateYachtCrew([FromForm] ObjectViewModel<YachtCrew> YachtCrew)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    YachtCrew.Object.Description = Methods.CoalesceString(YachtCrew.Object.Description);
                    Context.YachtCrew.Add(YachtCrew.Object);
                    await Context.SaveChangesAsync();
                    return RedirectToAction(nameof(YachtCrew));
                }
                catch (Exception exception)
                {
                    this.HandleException(exception);
                }
            }
            return LocalCreateYachtCrew(YachtCrew.Object);
        }
        public IActionResult DeleteYachtCrew(string id)
        {
            var YachtCrew = Context.YachtCrew
                /*Включение навигационных свойств*/
                .First(p => p.Id == int.Parse(id));
            return View("YachtCrewEditor", ObjectViewModelFactory<YachtCrew>.Delete(YachtCrew));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteYachtCrew([FromForm] ObjectViewModel<YachtCrew> YachtCrew)
        {
            try
            {
                Context.YachtCrew.Remove(YachtCrew.Object);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(YachtCrew));
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
            }
            return View("YachtCrewEditor", ObjectViewModelFactory<YachtCrew>.Delete(YachtCrew.Object));

        }
        #endregion

    }
}
