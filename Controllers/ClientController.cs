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
        
        #region Client
        public IActionResult Client()
        {
            var Client = Context.Client.OrderBy(p => p.Id);
            return View(Client);
        }

        public IActionResult EditClient(string id)
        {
            var Client = Context.Client.First(p => p.Id == int.Parse(id));
            return View("ClientEditor", ObjectViewModelFactory<Client>.Edit(Client));
        }

        [HttpPost]
        public async Task<IActionResult> EditClient([FromForm] ObjectViewModel<Client> staff)
        {
            if (ModelState.IsValid)
            {
                Context.Client.Update(staff.Object);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(Client));
            }

            return View("ClientEditor", ObjectViewModelFactory<Client>.Edit(staff.Object));
        }

        public IActionResult CreateClient()
        {
            var Client = new Client();
            return View("ClientEditor", ObjectViewModelFactory<Client>.Create(Client));
        }

        [HttpPost]
        public async Task<IActionResult> CreateClient([FromForm] ObjectViewModel<Client> staff)
        {
            if (ModelState.IsValid)
            {
                staff.Object.Registrydate = DateTime.Now;
                Context.Client.Add(staff.Object);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(Client));
            }

            return View("ClientEditor", ObjectViewModelFactory<Client>.Create(staff.Object));
        }
        public IActionResult DeleteClient(string id)
        {
            var Client = Context.Client.First(p => p.Id == int.Parse(id));
            return View("ClientEditor", ObjectViewModelFactory<Client>.Delete(Client));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteClient([FromForm] ObjectViewModel<Client> staff)
        {

            Context.Client.Remove(staff.Object);
            await Context.SaveChangesAsync();
            return RedirectToAction(nameof(Client));

        }
        #endregion

        #region Yachttype
        public IActionResult Yachttype()
        {
            var Yachttype = Context.Yachttype.OrderBy(p => p.Id);
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
            var Yachttype = new Yachttype();
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

        #region Yacht
        public IActionResult Yacht()
        {
            var Yacht = Context.Yacht
                .Include(p => p.Yachtowner)
                .Include(p => p.Type)
                .OrderBy(p => p.Id);
            return View(Yacht);
        }

        public IActionResult EditYacht(string id)
        {
            var Yacht = Context.Yacht
                .Include(p => p.Yachtowner)
                .Include(p => p.Type)
                .First(p => p.Id == int.Parse(id));
            var Model = ObjectViewModelFactory<Yacht>.Edit(Yacht);
            Model.Option[0] = Yacht.Rentable.Value;
            ViewData["Type"] = Context.Yachttype;
            ViewData["Yachtowner"] = Context.Client;
            return View("YachtEditor", Model);
        }

        [HttpPost]
        public async Task<IActionResult> EditYacht([FromForm] ObjectViewModel<Yacht> Yacht)
        {
            if (ModelState.IsValid)
            {
                Yacht.Object.Status = "[]";
                Yacht.Object.Rentable = Yacht.Option[0];
                Yacht.Object.Registrydate = DateTime.Now;
                Context.Yacht.Update(Yacht.Object);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(Yacht));
            }
            var Model = ObjectViewModelFactory<Yacht>.Edit(Yacht.Object);
            Model.Option[0] = Yacht.Object.Rentable.Value;
            ViewData["Type"] = Context.Yachttype;
            ViewData["Yachtowner"] = Context.Client;
            return View("YachtEditor", Model);
        }

        public IActionResult CreateYacht()
        {
            var Yacht = new Yacht();
            var Model = ObjectViewModelFactory<Yacht>.Create(Yacht);
            Yacht.Rentable = true;
            Model.Option[0] = true;
            ViewData["Type"] = Context.Yachttype;
            ViewData["Yachtowner"] = Context.Client;
            return View("YachtEditor", Model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateYacht([FromForm] ObjectViewModel<Yacht> Yacht)
        {
            if (ModelState.IsValid)
            {
                Yacht.Object.Status = "[]";
                Yacht.Object.Rentable = Yacht.Option[0];
                Yacht.Object.Registrydate = DateTime.Now;
                Context.Yacht.Add(Yacht.Object);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(Yacht));
            }
            var Model = ObjectViewModelFactory<Yacht>.Create(Yacht.Object);
            Model.Option[0] = true;
            ViewData["Type"] = Context.Yachttype;
            ViewData["Yachtowner"] = Context.Client;
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

            Context.Yacht.Remove(Yacht.Object);
            await Context.SaveChangesAsync();
            return RedirectToAction(nameof(Yacht));

        }
        #endregion

        #region Yachtleasetype
        public IActionResult Yachtleasetype()
        {
            var Yachtleasetype = Context.Yachtleasetype
                .OrderBy(p => p.Id);
            return View(Yachtleasetype);
        }

        public IActionResult DetailsYachtleasetype(string id)
        {
            var Yachtleasetype = Context.Yachtleasetype.First(p => p.Id == int.Parse(id));
            return View("_Details", new DetailsViewModel
            {
                Description = Yachtleasetype.Description,
                ButtonsViewModel = new EditorBottomButtonsViewModel
                {
                    BackAction = typeof(Yachtleasetype).Name
                }
            });
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

        public IActionResult DetailsYachtlease(string id)
        {
            var Yachtlease = Context.Yachtlease.First(p => p.Id == int.Parse(id));
            return View("_Details", new DetailsViewModel
            {
                Description = Yachtlease.Specials,
                ButtonsViewModel = new EditorBottomButtonsViewModel
                {
                    BackAction = typeof(Yachtlease).Name
                }
            });
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
                if (Yachtlease.Option[0]) Yachtlease.Object.Enddate = DateTime.Now;
                if( Yachtlease.Option[1]) Yachtlease.Object.Overallprice =
                        Context.Yachtleasetype.First(p => p.Id == Yachtlease.Object.Yachtleasetypeid).Price *
                    (Yachtlease.Object.Duration - Yachtlease.Object.Startdate).Days;
                Yachtlease.Object.Specials = Methods.IsStr(Yachtlease.Object.Specials) ? Yachtlease.Object.Specials : string.Empty;
                Context.Yachtlease.Update(Yachtlease.Object);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(Yachtlease));
            }
            var Model = ObjectViewModelFactory<Yachtlease>.Edit(Yachtlease.Object);
            return View("YachtleaseEditor", Model);
        }

        public IActionResult CreateYachtlease()
        {
            var Yachtlease = new Yachtlease ();
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
                Yachtlease.Object.Specials = Methods.IsStr(Yachtlease.Object.Specials) ? Yachtlease.Object.Specials : string.Empty;
                Yachtlease.Object.Startdate = DateTime.Now;
                Yachtlease.Object.Overallprice = 
                    Context.Yachtleasetype.First(p => p.Id == Yachtlease.Object.Yachtleasetypeid).Price * 
                    (Yachtlease.Object.Duration - Yachtlease.Object.Startdate).Days;
                Context.Yachtlease.Add(Yachtlease.Object);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(Yachtlease));
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
            Context.Yachtlease.Remove(Yachtlease.Object);
            await Context.SaveChangesAsync();
            return RedirectToAction(nameof(Yachtlease));

        }
        #endregion
    }
}
