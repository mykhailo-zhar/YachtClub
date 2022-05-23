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
    [Authorize(Policy = "EReq")]
    public class RepairStorekeeperCommonController : Controller
    {
        public DataContext Context;
        public RepairStorekeeperCommonController(DataContext context)
        {
            Context = context;
        }
        #region Extradationrequest
       
        public IActionResult Extradationrequest()
        {
            if (User.Role() == RolesReadonly.Storekeeper || User.Role() == RolesReadonly.DB_Admin)
            {
                var Extradationrequest = Context.Extradationrequest
                    .Include(p => p.Repair)
                    .Include(p => p.Staff)
                        .ThenInclude(p => p.Staff)
                    .Include(p => p.MaterialNavigation)
                        .ThenInclude(p => p.Type)
                    .Join(
                        Context.Availableresources,
                        e => e.Material,
                        a => a.Material,
                        (e, a) => new ExtradationRequestAvalilableViewModel
                        {
                            Extradationrequest = e,
                            Count = a.Count,
                            Format = a.Format,
                            LeadRight = false
                        }
                       )
                    .ToList()
                    .OrderByDescending(p => Methods.ExtradationStatusPrio(p.Extradationrequest.Status))
                    .ThenByDescending(p => p.Extradationrequest.Startdate);
                return View(Extradationrequest);
            }
            else
            {
                var Extradationrequest = Context.Extradationrequest
                    .Where(p => Context.LeadRepairMan(p.Repairid, User.PersonId()))
                    .Include(p => p.Repair)
                    .Include(p => p.Staff)
                        .ThenInclude(p => p.Staff)
                    .Include(p => p.MaterialNavigation)
                        .ThenInclude(p => p.Type)
                    .Join(
                        Context.Availableresources,
                        e => e.Material,
                        a => a.Material,
                        (e, a) => new ExtradationRequestAvalilableViewModel
                        {
                            Extradationrequest = e,
                            Count = a.Count,
                            Format = a.Format
                        }
                       )
                    .ToList()
                    .OrderByDescending(p => Methods.ExtradationStatusPrio(p.Extradationrequest.Status))
                    .ThenByDescending(p => p.Extradationrequest.Startdate);
                return View(Extradationrequest);
            }

        }

        private IActionResult LocalEditExtradationrequest(string id, Extradationrequest Extradationrequest = null)
        {
            Extradationrequest = Extradationrequest ?? Context.Extradationrequest
               .Include(p => p.Staff)
                    .ThenInclude(p => p.Staff)
                .Include(p => p.MaterialNavigation)
                    .ThenInclude(p => p.Type)
               .First(p => p.Id == int.Parse(id));

            ViewData["Material"] = Context.Material.Include(p => p.Type).ToList();
            var Model = ObjectViewModelFactory<Extradationrequest>.Edit(Extradationrequest);
            return View("ExtradationrequestEditor", Model);
        }

        public IActionResult EditExtradationrequest(string id) => LocalEditExtradationrequest(id);

        [HttpPost]
        public IActionResult EditExtradationrequest([FromForm] ObjectViewModel<Extradationrequest> Extradationrequest)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var OBJ = Context.Extradationrequest.First(p => p.Id == Extradationrequest.Object.Id);
                    OBJ.Duration = Extradationrequest.Object.Duration;
                    OBJ.Description = Methods.CoalesceString(Extradationrequest.Object.Description);
                    OBJ.Enddate = Extradationrequest.Object.Enddate;
                    OBJ.Status = Extradationrequest.Object.Status;
                    Context.SaveChanges();
                    return RedirectToAction(nameof(Extradationrequest));
                }
                catch (Exception exception)
                {
                    this.HandleException(exception);
                }
                if (ModelState.IsValid)
                {
                    return RedirectToAction(nameof(Extradationrequest));
                }
                return LocalEditExtradationrequest($"{Extradationrequest.Object.Id}", Extradationrequest.Object);
            }

            return LocalEditExtradationrequest($"{Extradationrequest.Object.Id}", Extradationrequest.Object);
        }

        
        private IActionResult LocalCreateExtradationrequest(Extradationrequest Extradationrequest = null, int repairid = 0)
        {
            var staff = Context.RepairStaff.Where(p => p.Staffid == User.PersonId()).First();
            Extradationrequest = Extradationrequest ?? new Extradationrequest { Repairid = repairid, Duration = DateTime.Now, Staffid = staff.Id };
            ViewData["Repair"] = Context.Repair.ToList();
            ViewData["Material"] = Context.Material.Include(p => p.Type).ToList();
            var Model = ObjectViewModelFactory<Extradationrequest>.Create(Extradationrequest);
            return View("ExtradationrequestEditor", Model);
        }
        [Authorize(Policy = "Repair")]
        public IActionResult CreateExtradationrequest(int repairid) => LocalCreateExtradationrequest(null, repairid);

        [HttpPost]
        [Authorize(Policy = "Repair")]
        public async Task<IActionResult> CreateExtradationrequest([FromForm] ObjectViewModel<Extradationrequest> Extradationrequest)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Extradationrequest.Option[0]) Extradationrequest.Object.Status = "Done";
                    Extradationrequest.Object.Startdate = DateTime.Now;
                    Context.Extradationrequest.Add(Extradationrequest.Object);
                    await Context.SaveChangesAsync();
                    return RedirectToAction(nameof(Extradationrequest));
                }
                catch (Exception exception)
                {
                    this.HandleException(exception);
                }
                if (ModelState.IsValid)
                {
                    return RedirectToAction(nameof(Extradationrequest));
                }
                return LocalCreateExtradationrequest(Extradationrequest.Object);
            }
            return LocalCreateExtradationrequest(Extradationrequest.Object);
        }
        public IActionResult DeleteExtradationrequest(string id)
        {
            var Extradationrequest = Context.Extradationrequest
                .First(p => p.Id == int.Parse(id));
            return View("ExtradationrequestEditor", ObjectViewModelFactory<Extradationrequest>.Delete(Extradationrequest));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteExtradationrequest([FromForm] ObjectViewModel<Extradationrequest> Extradationrequest)
        {
            try
            {
                Context.Extradationrequest.Remove(Extradationrequest.Object);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(Extradationrequest));
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
            }
            if (ModelState.IsValid)
            {
                return RedirectToAction(nameof(Extradationrequest));
            }
            return View("ExtradationrequestEditor", ObjectViewModelFactory<Extradationrequest>.Delete(Extradationrequest.Object));
        }
        #endregion
    }
}
