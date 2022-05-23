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
        public IActionResult DetailsYachttest(string id)
        {
            var Yachttest = Context.Yachttest.First(p => p.Id == int.Parse(id));
            return View("_Details", new DetailsViewModel
            {
                Description = Yachttest.Results,
                ButtonsViewModel = new EditorBottomButtonsViewModel
                {
                    BackAction = typeof(Yachttest).Name
                }
            });
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
                 Date = DateTime.Now
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
                ;
            return View(Repair);
        }

        private IActionResult AEditRepair(string id, Repair Repair = null)
        {
            Repair = Repair ?? Context.Repair
               .Include(p => p.Yacht)
                   .ThenInclude(p => p.Type)
               .First(p => p.Id == int.Parse(id));
            ViewData["Yacht"] = Context.Yacht.Include(p => p.Type);
            var Model = ObjectViewModelFactory<Repair>.Edit(Repair);
            return View("RepairEditor", Model);
        }
        public IActionResult EditRepair(string id) => AEditRepair(id);

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
                if (ModelState.IsValid)
                {
                    return RedirectToAction(nameof(Repair));
                }
                return AEditRepair($"{Repair.Object.Id}", Repair.Object);
            }
            return AEditRepair($"{Repair.Object.Id}", Repair.Object);
        }

        private IActionResult LocalCreateRepair(Repair Repair = null) {
            Repair = Repair ?? new Repair { 
                Duration = DateTime.Now,
                Personnel = 1
            };
            Repair.Startdate = DateTime.Now;
            ViewData["Yacht"] = Context.Yacht.Include(p => p.Type).Where(p => Context.Busyyacht.Any(y => !y.C && !y.E && !y.R && y.Val && y.Id == p.Id));
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
                if (ModelState.IsValid)
                {
                    return RedirectToAction(nameof(Repair));
                }
                return LocalCreateRepair(Repair.Object);
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
            if (ModelState.IsValid)
            {
                return RedirectToAction(nameof(Repair));
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
                if (ModelState.IsValid)
                {
                    return RedirectToAction(nameof(RepairMen));
                }
                return LocalCreateRepairMen(RepairMen.Object);
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

    }
}
