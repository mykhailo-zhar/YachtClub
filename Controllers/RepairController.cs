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
    [Authorize(Policy = "Repair")]
    public class RepairController : Controller
    {
        public DataContext Context;
        public RepairController(DataContext context)
        {
            Context = context;
        }

        #region Yachttest
        public IActionResult Yachttest()
        {
            var Yachttest = Context.Yachttest
                .Include(p => p.Staff)
                     .ThenInclude(p => p.Staff)
                        .Where(p => Context.RepairStaff.Any(s => s.Id == p.Staffid))
                .Include(p => p.Yacht)
                    .ThenInclude(p => p.Type)
                .OrderByDescending(p => p.Date);
            return View(Yachttest);
        }
        private IActionResult ACreateYachttest(Yachttest Yachttest)
        {
            ViewData["StaffPosition"] = Context.StaffPosition.FromSqlRaw(@"select * from Repair_Staff").Include(p => p.Yachttest);
            ViewData["Yacht"] = Context.Yacht.Include(p => p.Type).ToList();
            return View("YachttestEditor", ObjectViewModelFactory<Yachttest>.Create(Yachttest));
        }
        public IActionResult CreateYachttest()
        {
            var Yachttest = new Yachttest { 
                 Date = DateTime.Now,
                 Staffid = Context.RepairStaff.First(p => p.Staffid == User.PersonId()).Id
            };
            return ACreateYachttest(Yachttest);
        }

        [HttpPost]
        public async Task<IActionResult> CreateYachttest([FromForm] ObjectViewModel<Yachttest> Yachttest)
        {
            if (ModelState.IsValid)
            {
                Yachttest.Object.Date = DateTime.Now;
                Context.Yachttest.Add(Yachttest.Object);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(Yachttest));
            }
            return ACreateYachttest(Yachttest.Object);
        }
        public IActionResult DeleteYachttest(string id)
        {
            var Yachttest = Context.Yachttest.First(p => p.Id == int.Parse(id));
            return View("YachttestEditor", ObjectViewModelFactory<Yachttest>.Delete(Yachttest));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteYachttest([FromForm] ObjectViewModel<Yachttest> Yachttest)
        {

            Context.Yachttest.Remove(Yachttest.Object);
            await Context.SaveChangesAsync();
            return RedirectToAction(nameof(Yachttest));

        }
        #endregion

        #region Repair
        public IActionResult Repair()
        {
            var Repair = Context.Repair
                .Include(p => p.Yacht)
                    .ThenInclude(p => p.Type)
                .Where(l => Context.HasRepairMan(l.Id, User.PersonId()))
                .OrderBy(p => p.Id)
                .Select(p => new Repair_MenViewModel { 
                    Repair = p,
                    RepairMen = Context.RepairMen
                    .Include(z => z.Staff)
                        .ThenInclude(z => z.Staff)
                    .Where(r => r.Repairid == p.Id)
                    .ToList(),
                    Extradationrequests = Context.Extradationrequest
                    .Include(r => r.MaterialNavigation)
                         .ThenInclude(r => r.Type)
                    .Where(r => r.Repairid == p.Id),
                    LeadRight = Context.LeadRepairMan(p.Id, User.PersonId())
                }
                )
                .ToList()
                .OrderByDescending(p => Methods.RepairStatusPrio(p.Repair.Status))
                .ThenByDescending(p => p.Repair.Startdate)
                ;
            return View(Repair);
        }

        public void RepairConfigureViewBag()
        {
            ViewData["Yacht"] = Context.Yacht.Include(p => p.Type).Where(p => Context.Busyyacht.Any(y => !y.C && !y.E && !y.R && y.Val && y.Id == p.Id));
        }

        private IActionResult LocalEditRepair(string id, Repair Repair = null)
        {
            Repair = Repair ?? Context.Repair
               .Include(p => p.Yacht)
                   .ThenInclude(p => p.Type)
               .First(p => p.Id == int.Parse(id));
            RepairConfigureViewBag();
            var Model = ObjectViewModelFactory<Repair>.Edit(Repair);
            return View("RepairEditor", Model);
        }
        public IActionResult EditRepair(string id) => LocalEditRepair(id);

        [HttpPost]
        public async Task<IActionResult> EditRepair([FromForm] ObjectViewModel<Repair> Repair)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Repair.Object.Description = Methods.CoalesceString(Repair.Object.Description);
                    Context.Repair.Update(Repair.Object);
                    await Context.SaveChangesAsync();
                    return RedirectToAction(nameof(Repair));
                }
                catch (Exception exception)
                {
                    this.HandleException(exception);
                }
            }
            return LocalEditRepair($"{Repair.Object.Id}", Repair.Object);
        }

        private IActionResult LocalCreateRepair(Repair Repair = null) {
            Repair = Repair ?? new Repair { 
                Duration = DateTime.Now,
                Personnel = 1
            };
            Repair.Startdate = DateTime.Now;
            RepairConfigureViewBag();
            var Model = ObjectViewModelFactory<Repair>.Create(Repair);
            return View("RepairEditor", Model);
        }

        public IActionResult CreateRepair() => LocalCreateRepair(null);

        [HttpPost]
        public async Task<IActionResult> CreateRepair([FromForm] ObjectViewModel<Repair> Repair)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = Context.Database.BeginTransaction())
                {
                    try
                    {
                        Repair.Object.Startdate = default;
                        Repair.Object.Status = default;
                        Context.Repair.Add(Repair.Object);
                        await Context.SaveChangesAsync();
                        await Context.Database.ExecuteSqlRawAsync($"call populaterepair_men({Repair.Object.Id},{User.PersonId()});");
                        transaction.Commit();
                        return RedirectToAction(nameof(Repair));
                    }
                    catch (Exception exception)
                    {
                        this.HandleException(exception);
                        transaction.Rollback();
                    }
                }
            }
            return LocalCreateRepair(Repair.Object);
        }
        public IActionResult DeleteRepair(string id)
        {
            var Repair = Context.Repair
                .First(p => p.Id == int.Parse(id));
            return View("RepairEditor", ObjectViewModelFactory<Repair>.Delete(Repair));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRepair([FromForm] ObjectViewModel<Repair> Repair)
        {
            try
            {
                Context.Repair.Remove(Repair.Object);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(Repair));
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
            }
            return View("RepairEditor", ObjectViewModelFactory<Repair>.Delete(Repair.Object));
        }
        #endregion

        #region  RepairMen
        private IActionResult LocalCreateRepairMen(RepairMen RepairMen = null, int RepairID = 0)
        {
            RepairMen = RepairMen ?? new RepairMen { 
                Repairid = RepairID
            };
            ViewData["StaffPosition"] = Context.RepairStaff
                .Where(p => !Context.RepairMen.Where(a => a.Repairid == RepairMen.Repairid).Any(a => a.Staffid == p.Id))
                .Include(p => p.Staff)
                .ToList();
            var Model = ObjectViewModelFactory<RepairMen>.Create(RepairMen);
            return View("RepairMenEditor", Model);
        }

        public IActionResult CreateRepairMen(int repairid) => LocalCreateRepairMen(null,repairid);

        [HttpPost]
        public async Task<IActionResult> CreateRepairMen([FromForm] ObjectViewModel<RepairMen> RepairMen)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Context.RepairMen.Add(RepairMen.Object);
                    await Context.SaveChangesAsync();
                    return RedirectToAction(nameof(Repair));
                }
                catch (Exception exception)
                {
                    this.HandleException(exception);
                }
            }
            return LocalCreateRepairMen(RepairMen.Object);
        }
        public IActionResult DeleteRepairMen(string id)
        {
            var RepairMen = Context.RepairMen
                /*Включение навигационных свойств*/
                .First(p => p.Id == int.Parse(id));
            return View("RepairMenEditor", ObjectViewModelFactory<RepairMen>.Delete(RepairMen));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRepairMen([FromForm] ObjectViewModel<RepairMen> RepairMen)
        {
            try
            {
                Context.RepairMen.Remove(RepairMen.Object);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(Repair));
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
            }
            if (ModelState.IsValid)
            {
                return RedirectToAction(nameof(RepairMen));
            }
            return View("RepairMenEditor", ObjectViewModelFactory<RepairMen>.Delete(RepairMen.Object));
        }
        #endregion

        #region RepairStaff

        public IActionResult RepairStaff()
        {
            var RepairStaff = Context.RepairStaff
                     .Include(p => p.Staff)
                     .Include(p => p.Position)
                .OrderBy(p => p.Id).ToList();

            return View(RepairStaff);
        }

        #endregion

        [HttpGet]
        public IActionResult RepairSearch(RepairInfo Object)
        {
            Object = !Object.Flag ? new RepairInfo
            {
                Flag = true,
                LikeName = string.Empty,
                LikeSurname = string.Empty,
                LikeEmail = string.Empty,
                LikePhone = string.Empty,
                LikeYName = string.Empty,
                LikeYType = string.Empty,
                Active = true
            }
            : Object
            ;

            Object.Yachts = Context.Yacht.Select(p => p.Name).Distinct().OrderBy(p => p);
            Object.YachtType = Context.Yachttype.Select(p => p.Name).Distinct().OrderBy(p => p);

            Object.Repairs = Context.RepairSearch(
               Object.LikeName,
               Object.LikeSurname,
               Object.LikePhone,
               Object.LikeEmail,
               Object.LikeYName,
               Object.LikeYType,
               Object.Active)
                .OrderByDescending(p => p.Enddate ?? DateTime.Now)
               .ToList();
            return View(Object);
        }

    }
}
