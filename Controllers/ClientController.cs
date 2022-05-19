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
    public class ClientController : Controller
    {
        public DataContext Context;
        public ClientController(DataContext context)
        {
            Context = context;
        }

        #region Person
        public IActionResult Person()
        {
            var Person = Context.Person.Where(p => !p.Staffonly).OrderBy(p => p.Id);
            return View(Person);
        }

        public IActionResult EditPerson(string id, bool? staffonly )
        {

            ViewBag.FromStaff = staffonly ?? false;

            var Person = Context.Person.First(p => p.Id == int.Parse(id));
            return View("PersonEditor", ObjectViewModelFactory<Person>.Edit(Person));
        }

        [HttpPost]
        public async Task<IActionResult> EditPerson([FromForm] ObjectViewModel<Person> Person)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var obj = Context.Person.First(p => p.Id == Person.Object.Id);
                    obj.Name = Person.Object.Name;
                    obj.Surname = Person.Object.Surname;
                    obj.Phone = Person.Object.Phone;
                    obj.Email = Person.Object.Email;
                    obj.Sex = Person.Object.Sex;
                    obj.Staffonly = Person.Object.Staffonly;
                    obj.Birthdate = Person.Object.Birthdate;
                    await Context.SaveChangesAsync();
                    return Person.Object.Staffonly ? RedirectToAction(nameof(Staff), nameof(Staff)) : RedirectToAction(nameof(Person));
                }
                catch (Exception exception)
                {
                    this.HandleException(exception);
                }
            }

            return View("PersonEditor", ObjectViewModelFactory<Person>.Edit(Person.Object));
        }

        public IActionResult CreatePerson(bool? staffonly = false)
        {
            ViewBag.FromStaff = staffonly ?? false;
            var Person = new Person
            {
                Birthdate = DateTime.Now,
                Staffonly = staffonly ?? false
            };
            return View("PersonEditor", ObjectViewModelFactory<Person>.Create(Person));
        }

        [HttpPost]
        public async Task<IActionResult> CreatePerson([FromForm] ObjectViewModel<Person> Person)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Context.Person.Add(Person.Object);
                    await Context.SaveChangesAsync();
                    return RedirectToAction(nameof(Person));
                }
                catch (Exception exception)
                {
                    this.HandleException(exception);
                }
            }

            return View("PersonEditor", ObjectViewModelFactory<Person>.Create(Person.Object));
        }
        public IActionResult DeletePerson(string id)
        {
            var Person = Context.Person.First(p => p.Id == int.Parse(id));
            return View("PersonEditor", ObjectViewModelFactory<Person>.Delete(Person));
        }

        [HttpPost]
        public async Task<IActionResult> DeletePerson([FromForm] ObjectViewModel<Person> Person)
        {
            try
            {
                Context.Person.Remove(Person.Object);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(Person));
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
            }

            return View("PersonEditor", ObjectViewModelFactory<Person>.Delete(Person.Object));

        }
        #endregion

        #region Yacht
        public IActionResult Yacht()
        {
            var Yacht = Context.Yacht
                .Include(p => p.Yachtowner)
                .Include(p => p.Type)
                .OrderBy(p => p.Id)
                .Select(
                    p => new YachtWithStatusViewModel
                    {
                        Yacht = p,
                        Status = Context.YachtsStatus(p.Id)
                    }
                );
            return View(Yacht);
        }

        public IActionResult EditYacht(string id)
        {
            var Yacht = Context.Yacht
                .Include(p => p.Yachtowner)
                .Include(p => p.Type)
                .First(p => p.Id == int.Parse(id));
            var Model = ObjectViewModelFactory<Yacht>.Edit(Yacht);
            ViewData["Type"] = Context.Yachttype;
            ViewData["Yachtowner"] = Context.Person.Where(p => !p.Staffonly);
            return View("YachtEditor", Model);
        }

        [HttpPost]
        public async Task<IActionResult> EditYacht([FromForm] ObjectViewModel<Yacht> Yacht)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Yacht.Object.Registrydate = DateTime.Now;
                    Context.Yacht.Update(Yacht.Object);
                    await Context.SaveChangesAsync();
                    return RedirectToAction(nameof(Yacht));
                }
                catch (Exception exception)
                {
                    this.HandleException(exception);
                }
            }
            var Model = ObjectViewModelFactory<Yacht>.Edit(Yacht.Object);
            ViewData["Type"] = Context.Yachttype;
            ViewData["Yachtowner"] = Context.Person.Where(p => !p.Staffonly);
            return View("YachtEditor", Model);


        }

        public IActionResult CreateYacht()
        {
            var Yacht = new Yacht { 
                Description = "",
                Rentable = false
            };
            var Model = ObjectViewModelFactory<Yacht>.Create(Yacht);
            ViewData["Type"] = Context.Yachttype;
            ViewData["Yachtowner"] = Context.Person.Where(p => !p.Staffonly); ;
            return View("YachtEditor", Model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateYacht([FromForm] ObjectViewModel<Yacht> Yacht)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Yacht.Object.Registrydate = DateTime.Now;
                    Context.Yacht.Add(Yacht.Object);
                    await Context.SaveChangesAsync();
                    return RedirectToAction(nameof(Yacht));
                }
                catch (Exception exception)
                {
                    this.HandleException(exception);
                }
            }
            var Model = ObjectViewModelFactory<Yacht>.Create(Yacht.Object);
            ViewData["Type"] = Context.Yachttype;
            ViewData["Yachtowner"] = Context.Person.Where(p => !p.Staffonly);
            return View("YachtEditor", Model);
        }
        public IActionResult DeleteYacht(string id)
        {
            var Yacht = Context.Yacht
                .Include(p => p.Yachtowner)
                .Include(p => p.Type)
                .First(p => p.Id == int.Parse(id));
            return View("YachtEditor", ObjectViewModelFactory<Yacht>.Delete(Yacht));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteYacht([FromForm] ObjectViewModel<Yacht> Yacht)
        {
            try
            {
                Context.Yacht.Remove(Yacht.Object);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(Yacht));
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
            }
            return View("YachtEditor", ObjectViewModelFactory<Yacht>.Delete(Yacht.Object));
        }
        #endregion


        //TODO: Перенести во вкладку владельца

        #region Yachtleasetype
        public IActionResult Yachtleasetype()
        {
            var Yachtleasetype = Context.Yachtleasetype
                .OrderBy(p => p.Id);
            return View(Yachtleasetype);
        }
        public IActionResult EditYachtleasetype(string id)
        {
            var Yachtleasetype = Context.Yachtleasetype
                .First(p => p.Id == int.Parse(id));
            var Model = ObjectViewModelFactory<Yachtleasetype>.Edit(Yachtleasetype);
            return View("YachtleasetypeEditor", Model);
        }

        [HttpPost]
        public async Task<IActionResult> EditYachtleasetype([FromForm] ObjectViewModel<Yachtleasetype> Yachtleasetype)
        {
            if (ModelState.IsValid)
            {
                Context.Yachtleasetype.Update(Yachtleasetype.Object);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(Yachtleasetype));
            }
            var Model = ObjectViewModelFactory<Yachtleasetype>.Edit(Yachtleasetype.Object);
            return View("YachtleasetypeEditor", Model);
        }

        public IActionResult CreateYachtleasetype()
        {
            var Yachtleasetype = new Yachtleasetype();
            var Model = ObjectViewModelFactory<Yachtleasetype>.Create(Yachtleasetype);
            return View("YachtleasetypeEditor", Model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateYachtleasetype([FromForm] ObjectViewModel<Yachtleasetype> Yachtleasetype)
        {
            if (ModelState.IsValid)
            {

                Context.Yachtleasetype.Add(Yachtleasetype.Object);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(Yachtleasetype));
            }
            var Model = ObjectViewModelFactory<Yachtleasetype>.Create(Yachtleasetype.Object);
            return View("YachtleasetypeEditor", Model);
        }
        public IActionResult DeleteYachtleasetype(string id)
        {
            var Yachtleasetype = Context.Yachtleasetype
                .First(p => p.Id == int.Parse(id));
            return View("YachtleasetypeEditor", ObjectViewModelFactory<Yachtleasetype>.Delete(Yachtleasetype));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteYachtleasetype([FromForm] ObjectViewModel<Yachtleasetype> Yachtleasetype)
        {

            Context.Yachtleasetype.Remove(Yachtleasetype.Object);
            await Context.SaveChangesAsync();
            return RedirectToAction(nameof(Yachtleasetype));

        }
        #endregion

        //TODO: Исправить ошибки в договоре на яхты и пофиксить интерфейс
        #region Yachtlease
        public IActionResult Yachtlease()
        {
            var Yachtlease = Context.Yachtlease
                .Include(p => p.Yacht)
                    .ThenInclude(p => p.Type)
                .Include(p => p.Yachtleasetype)
                .OrderBy(p => p.Id);
            return View(Yachtlease);
        }
        public IActionResult EditYachtlease(string id)
        {
            var Yachtlease = Context.Yachtlease
                .Include(p => p.Yacht)
                    .ThenInclude(p => p.Type)
                .Include(p => p.Yachtleasetype)
                .First(p => p.Id == int.Parse(id));
            ViewData["Yacht"] = Context.Yacht.Include(p => p.Type);
            ViewData["Yachtleasetype"] = Context.Yachtleasetype;
            var Model = ObjectViewModelFactory<Yachtlease>.Edit(Yachtlease);
            return View("YachtleaseEditor", Model);
        }

        [HttpPost]
        public async Task<IActionResult> EditYachtlease([FromForm] ObjectViewModel<Yachtlease> Yachtlease)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Yachtlease.Option[0]) Yachtlease.Object.Enddate = DateTime.Now;
                    if (Yachtlease.Option[1]) Yachtlease.Object.Overallprice = Context.Yachtleasetype.First(p => p.Id == Yachtlease.Object.Yachtleasetypeid).Price *
                        (Yachtlease.Object.Duration - Yachtlease.Object.Startdate).Days;
                    Yachtlease.Object.Specials = Methods.IsStr(Yachtlease.Object.Specials) ? Yachtlease.Object.Specials : string.Empty;
                    Context.Yachtlease.Update(Yachtlease.Object);
                    await Context.SaveChangesAsync();
                    return RedirectToAction(nameof(Yachtlease));
                }
                catch (Exception exception)
                {
                    this.HandleException(exception);
                }
            }
            var Model = ObjectViewModelFactory<Yachtlease>.Edit(Yachtlease.Object);
            return View("YachtleaseEditor", Model);
        }

        public IActionResult CreateYachtlease()
        {
            var Yachtlease = new Yachtlease();
            ViewData["Yacht"] = Context.Yacht.Include(p => p.Type);
            ViewData["Yachtleasetype"] = Context.Yachtleasetype;
            var Model = ObjectViewModelFactory<Yachtlease>.Create(Yachtlease);
            return View("YachtleaseEditor", Model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateYachtlease([FromForm] ObjectViewModel<Yachtlease> Yachtlease)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Yachtlease.Object.Specials = Methods.IsStr(Yachtlease.Object.Specials) ? Yachtlease.Object.Specials : string.Empty;
                    Yachtlease.Object.Startdate = DateTime.Now;
                    Yachtlease.Object.Overallprice =
                        Context.Yachtleasetype.First(p => p.Id == Yachtlease.Object.Yachtleasetypeid).Price *
                        (Yachtlease.Object.Duration - Yachtlease.Object.Startdate).Days;
                    Context.Yachtlease.Add(Yachtlease.Object);
                    await Context.SaveChangesAsync();
                    return RedirectToAction(nameof(Yachtlease));
                }
                catch (Exception exception)
                {
                    this.HandleException(exception);
                }
            }
            var Model = ObjectViewModelFactory<Yachtlease>.Create(Yachtlease.Object);
            return View("YachtleaseEditor", Model);
        }
        public IActionResult DeleteYachtlease(string id)
        {
            var Yachtlease = Context.Yachtlease
                .First(p => p.Id == int.Parse(id));
            return View("YachtleaseEditor", ObjectViewModelFactory<Yachtlease>.Delete(Yachtlease));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteYachtlease([FromForm] ObjectViewModel<Yachtlease> Yachtlease)
        {
            try
            {
                Context.Yachtlease.Remove(Yachtlease.Object);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(Yachtlease));
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
            }
            return View("YachtleaseEditor", ObjectViewModelFactory<Yachtlease>.Delete(Yachtlease.Object));
        }
        #endregion

        #region Yachttype
        public IActionResult Yachttype()
        {
            var Yachttype = Context.Yachttype.Select(p => new MinCrewViewModel
            {
                Yachttype = p,
                MinCrew = Context.PositionYachttype
                    .Where(z => z.Yachttypeid == p.Id)
                    .Include(l => l.Position)
                    .ToList()
            });
            return View(Yachttype);
        }

        public IActionResult DetailsYachttype(string id)
        {
            var Yachttype = Context.Yachttype.First(p => p.Id == int.Parse(id));
            return View("_Details", new DetailsViewModel
            {
                Description = Yachttype.Description,
                ButtonsViewModel = new EditorBottomButtonsViewModel
                {
                    BackAction = typeof(Yachttype).Name
                }
            });
        }

        public IActionResult EditYachttype(string id)
        {
            var Yachttype = Context.Yachttype.First(p => p.Id == int.Parse(id));
            return View("YachttypeEditor", ObjectViewModelFactory<Yachttype>.Edit(Yachttype));
        }

        [HttpPost]
        public async Task<IActionResult> EditYachttype([FromForm] ObjectViewModel<Yachttype> staff)
        {
            if (ModelState.IsValid)
            {
                Context.Yachttype.Update(staff.Object);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(Yachttype));
            }

            return View("YachttypeEditor", ObjectViewModelFactory<Yachttype>.Edit(staff.Object));
        }

        public IActionResult CreateYachttype()
        {
            var Yachttype = new Yachttype { 
                Description = ""
            };
            return View("YachttypeEditor", ObjectViewModelFactory<Yachttype>.Create(Yachttype));
        }

        [HttpPost]
        public async Task<IActionResult> CreateYachttype([FromForm] ObjectViewModel<Yachttype> staff)
        {
            if (ModelState.IsValid)
            {
                Context.Yachttype.Add(staff.Object);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(Yachttype));
            }

            return View("YachttypeEditor", ObjectViewModelFactory<Yachttype>.Create(staff.Object));
        }
        public IActionResult DeleteYachttype(string id)
        {
            var Yachttype = Context.Yachttype.First(p => p.Id == int.Parse(id));
            return View("YachttypeEditor", ObjectViewModelFactory<Yachttype>.Delete(Yachttype));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteYachttype([FromForm] ObjectViewModel<Yachttype> staff)
        {

            Context.Yachttype.Remove(staff.Object);
            await Context.SaveChangesAsync();
            return RedirectToAction(nameof(Yachttype));

        }
        #endregion

        #region PositionYachttype
        private IActionResult LocalEditPositionYachttype(string ytid, string pid, PositionYachttype Object = null)
        {
            Object = Object ?? Context.PositionYachttype

               .First(p => p.Yachttypeid == int.Parse(ytid) && p.Positionid == int.Parse(pid));
            ViewData["Type"] = Context.Position.Where(p => p.Crewposition).ToList();
            var Model = ObjectViewModelFactory<PositionYachttype>.Edit(Object);
            return View("PositionYachttypeEditor", Model);
        }

        public IActionResult EditPositionYachttype(string ytid, string pid) => LocalEditPositionYachttype(ytid, pid);

        [HttpPost]
        public async Task<IActionResult> EditPositionYachttype([FromForm] ObjectViewModel<PositionYachttype> PositionYachttype)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //PositionYachttype.Object.Description = Methods.CoalesceString(PositionYachttype.Object.Description);
                    Context.PositionYachttype.Update(PositionYachttype.Object);
                    await Context.SaveChangesAsync();
                    return RedirectToAction(nameof(Yachttype));
                }
                catch (Exception exception)
                {
                    this.HandleException(exception);
                }
                return LocalEditPositionYachttype($"{PositionYachttype.Object.Yachttypeid}", $"{PositionYachttype.Object.Positionid}", PositionYachttype.Object);
            }

            return LocalEditPositionYachttype($"{PositionYachttype.Object.Yachttypeid}", $"{PositionYachttype.Object.Positionid}", PositionYachttype.Object);
        }

        private IActionResult LocalCreatePositionYachttype(PositionYachttype PositionYachttype = null, int id = 0)
        {
            PositionYachttype = PositionYachttype ?? new PositionYachttype { 
                Yachttypeid = id,
                Count = 1
            };
            ViewData["Type"] = Context.Position.Where(p => p.Crewposition && !Context.PositionYachttype.Where(z => z.Yachttypeid == PositionYachttype.Yachttypeid).Select(z => z.Positionid).Contains(p.Id)).ToList();
            var Model = ObjectViewModelFactory<PositionYachttype>.Create(PositionYachttype);
            return View("PositionYachttypeEditor", Model);
        }

        public IActionResult CreatePositionYachttype(int ytid) => LocalCreatePositionYachttype(null, ytid);

        [HttpPost]
        public async Task<IActionResult> CreatePositionYachttype([FromForm] ObjectViewModel<PositionYachttype> PositionYachttype)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //PositionYachttype.Object.Description = Methods.CoalesceString(PositionYachttype.Object.Description);
                    Context.PositionYachttype.Add(PositionYachttype.Object);
                    await Context.SaveChangesAsync();
                    return RedirectToAction(nameof(Yachttype));
                }
                catch (Exception exception)
                {
                    this.HandleException(exception);
                }
                return LocalCreatePositionYachttype(PositionYachttype.Object);
            }
            return LocalCreatePositionYachttype(PositionYachttype.Object);
        }


        public IActionResult DeletePositionYachttype(string ytid, string pid)
        {
            var PositionYachttype = Context.PositionYachttype
                 .First(p => p.Yachttypeid == int.Parse(ytid) && p.Positionid == int.Parse(pid));
            return View("PositionYachttypeEditor", ObjectViewModelFactory<PositionYachttype>.Delete(PositionYachttype));
        }

        [HttpPost]
        public async Task<IActionResult> DeletePositionYachttype([FromForm] ObjectViewModel<PositionYachttype> PositionYachttype)
        {
            try
            {
                Context.PositionYachttype.Remove(PositionYachttype.Object);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(Yachttype));
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
            }
            return View("PositionYachttypeEditor", ObjectViewModelFactory<PositionYachttype>.Delete(PositionYachttype.Object));

        }
        #endregion

    }
}
