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
                        .Where(p => Context.RepairStaff.Select(p => p.Id).Contains(p.Staffid))
                .Include(p => p.Yacht)
                    .ThenInclude(p => p.Type)
                .OrderBy(p => p.Id);
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

        public IActionResult CreateYachttest()
        {
            var Yachttest = new Yachttest();
            ViewData["StaffPosition"] = Context.StaffPosition.FromSqlRaw(@"select * from Repairmen").Include(p => p.Staff);
            ViewData["Yacht"] = Context.Yacht.Include(p => p.Type).ToList();
            return View("YachttestEditor", ObjectViewModelFactory<Yachttest>.Create(Yachttest));
        }

        [HttpPost]
        public async Task<IActionResult> CreateYachttest([FromForm] ObjectViewModel<Yachttest> staff)
        {
            if (ModelState.IsValid)
            {
                staff.Object.Date = DateTime.Now;
                Context.Yachttest.Add(staff.Object);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(Yachttest));
            }
            ViewData["StaffPosition"] = Context.StaffPosition.FromSqlRaw(@"select * from Repairmen").Include(p => p.Staff);
            ViewData["Yacht"] = Context.Yacht.Include(p => p.Type).ToList();
            return View("YachttestEditor", ObjectViewModelFactory<Yachttest>.Create(staff.Object));
        }
        public IActionResult DeleteYachttest(string id)
        {
            var Yachttest = Context.Yachttest.First(p => p.Id == int.Parse(id));
            return View("YachttestEditor", ObjectViewModelFactory<Yachttest>.Delete(Yachttest));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteYachttest([FromForm] ObjectViewModel<Yachttest> staff)
        {

            Context.Yachttest.Remove(staff.Object);
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
                .OrderBy(p => p.Id);
            return View(Repair);
        }

        public IActionResult DetailsRepair(string id)
        {
            var Repair = Context.Repair.First(p => p.Id == int.Parse(id));
            return View("_Details", new DetailsViewModel
            {
                Description = Repair.Description,
                ButtonsViewModel = new EditorBottomButtonsViewModel
                {
                    BackAction = typeof(Repair).Name
                }
            });
        }

        private IActionResult LocalEditRepair(string id, Repair Repair = null)
        {
            Repair = Repair ?? Context.Repair
               .Include(p => p.Yacht)
                   .ThenInclude(p => p.Type)
               .First(p => p.Id == int.Parse(id));
            ViewData["Yacht"] = Context.Yacht.Include(p => p.Type);
            var Model = ObjectViewModelFactory<Repair>.Edit(Repair);
            return View("RepairEditor", Model);
        }
        public IActionResult EditRepair(string id) => LocalEditRepair(id);

        [HttpPost]
        public async Task<IActionResult> EditRepair([FromForm] ObjectViewModel<Repair> Repair)
        {
            if (ModelState.IsValid)
            {

                if (Repair.Option[0]) {
                    Repair.Object.Enddate = DateTime.Now;
                    Repair.Object.Status = "Завершен";
                }
                Repair.Object.Description = Methods.CoalesceString(Repair.Object.Description);
                Context.Repair.Update(Repair.Object);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(Repair));
            }
           
            return LocalEditRepair($"{Repair.Object.Id}", Repair.Object);
        }

        private IActionResult LocalCreateRepair(Repair Repair = null) {
            Repair = Repair ?? new Repair();
            Repair.Startdate = DateTime.Now;
            ViewData["Yacht"] = Context.Yacht.Include(p => p.Type);
            var Model = ObjectViewModelFactory<Repair>.Create(Repair);
            return View("RepairEditor", Model);
        }

        public IActionResult CreateRepair() => LocalCreateRepair(null);

        [HttpPost]
        public async Task<IActionResult> CreateRepair([FromForm] ObjectViewModel<Repair> Repair)
        {
            if (ModelState.IsValid)
            {
                Repair.Object.Description = Methods.CoalesceString(Repair.Object.Description);
                Context.Repair.Add(Repair.Object);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(Repair));
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
            Context.Repair.Remove(Repair.Object);
            await Context.SaveChangesAsync();
            return RedirectToAction(nameof(Repair));

        }
        #endregion

        #region  RepairMen
        public IActionResult RepairMen()
        {
            var RepairMen = Context.RepairMen
                .Include(p => p.Staff)
                     .ThenInclude(p => p.Staff)
                        .Where(p => Context.RepairStaff.Select(p => p.Id).Contains(p.Staffid))
                .Include(p => p.Repair)
                .OrderBy(p => p.Id).ToList();

            return View(RepairMen);
        }

       
        private IActionResult LocalCreateRepairMen(RepairMen RepairMen = null)
        {
            RepairMen = RepairMen ?? new RepairMen();
            ViewData["StaffPosition"] = Context.StaffPosition.FromSqlRaw(@"select * from RepairMen").Include(p => p.Staff).ToList();
            ViewData["Repair"] = Context.Repair.ToList();
            var Model = ObjectViewModelFactory<RepairMen>.Create(RepairMen);
            return View("RepairMenEditor", Model);
        }

        public IActionResult CreateRepairMen() => LocalCreateRepairMen(null);

        [HttpPost]
        public async Task<IActionResult> CreateRepairMen([FromForm] ObjectViewModel<RepairMen> RepairMen)
        {
            if (ModelState.IsValid)
            {
                //RepairMen.Object.Description = Methods.CoalesceString(RepairMen.Object.Description);
                Context.RepairMen.Add(RepairMen.Object);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(RepairMen));
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
            Context.RepairMen.Remove(RepairMen.Object);
            await Context.SaveChangesAsync();
            return RedirectToAction(nameof(RepairMen));

        }
        #endregion

        #region RepairStaff

        public IActionResult RepairStaff()
        {
            var RepairStaff = Context.RepairStaff
                     .Include(p => p.Staff)
                        .Where(p => Context.RepairStaff.Select(p => p.Id).Contains(p.Id))
                     .Include(p => p.Position)
                .OrderBy(p => p.Id).ToList();

            return View(RepairStaff);
        }

        #endregion
    }
}
