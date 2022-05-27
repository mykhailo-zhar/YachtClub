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
    [Authorize(Policy = "CaptainCrew")]
    public class YachtCrewCommonController : Controller
    {
        public DataContext Context;
        public YachtCrewCommonController(DataContext context)
        {
            Context = context;
        }

        #region YachtCrew

        public IActionResult YachtCrew()
        {
            bool IsCaptain = User.IsInRole(RolesReadonly.Captain);
            if (!IsCaptain)
            {
                var Object = Context.YachtCrew
                    .Include(p => p.Yacht)
                        .ThenInclude(p => p.Type) 
                    .Include(p => p.Yacht)
                        .ThenInclude(p => p.Yachtowner)
                    .Include(p => p.Crew)
                        .ThenInclude(p => p.Position)
                    .Include(p => p.Crew)
                        .ThenInclude(p => p.Staff)
                    .Select(p => new YachtCrewStatusViewModel
                    {
                        Captain = p,
                        Status = Context.YachtsStatus(p.Yachtid),
                        CurrentCrew = Context.CountActiveCrew(p.Yachtid)
                    })
                    .OrderByDescending(p => p.Captain.Enddate ?? DateTime.Now)
                    .ThenBy(p => p.Captain.Crew.Positionid)
                    .ToList()
                    .GroupBy(p => p.Captain.Yacht)
                    .Select(p => new YachtCrewGroupingViewModel
                    {
                        Group = p,
                        AllowedOnlyCaptain = false
                    });
                return View(Object);
            }
            else
            {
                var Object = Context.YachtCrew
                   .Include(p => p.Yacht)
                       .ThenInclude(p => p.Type)
                   .Include(p => p.Crew)
                       .ThenInclude(p => p.Position)
                   .Include(p => p.Yacht)
                        .ThenInclude(p => p.Yachtowner)
                   .Include(p => p.Crew)
                       .ThenInclude(p => p.Staff)
                   .Where(p => p.Crew.Staffid == User.PersonId() && p.Crew.Position.Name == RolesReadonly.Captain)
                   .Where(p => p.Enddate == null || p.Crew.Position.Name != RolesReadonly.Captain)
                   .Select(p => new YachtCrewStatusViewModel
                   {
                       Captain = p,
                       Crew = Context.YachtCrew.Where(a => p.Id != a.Id && a.Yachtid == p.Yachtid && a.Enddate == null),
                       Status = Context.YachtsStatus(p.Yachtid)
                   })
                   .OrderByDescending(p => p.Captain.Enddate ?? DateTime.Now)
                   .ThenBy(p => p.Captain.Crew.Positionid)
                   .ToList()
                   .GroupBy(p => p.Captain.Yacht)
                   .Select(p => new YachtCrewGroupingViewModel
                   {
                       Group = p,
                       AllowedOnlyCaptain = true
                   });
                return View(Object);
            }

        }

        private void YachtCrewConfigureViewBag()
        {
            if (!User.IsInRole(RolesReadonly.Captain))
            {
                ViewBag.Crew = Context.StaffPosition
                .Include(p => p.Position)
                .Include(p => p.Staff)
                .Where(p => p.Position.Crewposition && p.Position.Name == RolesReadonly.Captain)
                .Where(p => p.Enddate == null)
                .Where(p => !Context.YachtCrew
                              .Where(y => y.Enddate == null)
                              .Any(o => o.Crewid == p.Id))
                ;

                ViewBag.Yacht = Context.Yacht
               .Include(p => p.Type)
               .Where(p => Context.Busyyacht.Any(b => b.Id == p.Id && b.Val))
               .Where(p => p.Type.Crewcapacity - Context.YachtCrew.Where(c => c.Enddate == null && c.Yachtid == p.Id).Count() > 0)
               ;
            }
            
            else
            {
                ViewBag.Crew = Context.StaffPosition
                .Include(p => p.Position)
                .Include(p => p.Staff)
                .Where(p => p.Position.Crewposition && p.Position.Name != RolesReadonly.Captain)
                .Where(p => p.Enddate == null)
                .Where(p => !Context.YachtCrew
                              .Where(y => y.Enddate == null)
                              .Any(o => o.Crewid == p.Id)
                              );

                ViewBag.Yacht = Context.Yacht
               .Include(p => p.Type)
               .Where(y => Context.YachtCrew
                                           .Include(p => p.Crew)
                                           .ThenInclude(p => p.Position)
                                           .Where(p => p.Crew.Position.Name == RolesReadonly.Captain && p.Crew.Staffid == User.PersonId())
                                           .Where(p => p.Enddate == null)
                                           .Any(p => p.Yachtid == y.Id)
                     )
               .Where(p => Context.Busyyacht.Any(b => b.Id == p.Id && b.Val))
               .Where(p => p.Type.Crewcapacity - Context.YachtCrew.Where(c => c.Enddate == null && c.Yachtid == p.Id).Count() > 0)
               ;
            }


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
