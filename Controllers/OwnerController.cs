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
    public class OwnerController : Controller
    {

        public DataContext Context;
        public OwnerController(DataContext context)
        {
            Context = context;
        }

        #region Event
        public IActionResult Event()
        {
            var Object = Context.Event.Select(e => new EventViewModel
                {
                    Event = e,
                    Winners = Context.Winner
                    .Include(p => p.Yacht)
                        .ThenInclude(p => p.Type)
                    .Where(p => p.Eventid == e.Id)
                    .OrderBy(p => p.Place)
                    .ToList()
                })
                .OrderByDescending(e => e.Event.Enddate ?? DateTime.Now)
                ;
            return View(Object);
        }

        private void EventConfigureViewBag()
        {
            //Для ViewBag
        }
      
        private IActionResult LocalEditEvent(string id, Event Event = null)
        {
            Event = Event ?? Context.Event
               /*Включение навигационных свойств*/
               .First(p => p.Id == int.Parse(id));
            /*Включение навигационных свойств*/
            var Model = ObjectViewModelFactory<Event>.Edit(Event);
            Model.Option[1] = Model.Object.Canhavewinners.Value;
            EventConfigureViewBag();
            return View("EventEditor", Model);
        }

        public IActionResult EditEvent(string id) => LocalEditEvent(id);

        [HttpPost]
        public async Task<IActionResult> EditEvent([FromForm] ObjectViewModel<Event> Event)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Event.Option[0])
                    {
                        Event.Object.Enddate = DateTime.Now;
                    }
                    Event.Object.Canhavewinners = Event.Option[1];
                    Event.Object.Description = Methods.CoalesceString(Event.Object.Description);
                    Context.Event.Update(Event.Object);
                    await Context.SaveChangesAsync();
                    return RedirectToAction(nameof(Event));
                }
                catch (Exception exception)
                {
                    this.HandleException(exception);
                }
            }

            return LocalEditEvent($"{Event.Object.Id}", Event.Object);
        }

        private IActionResult LocalCreateEvent(Event Event = null)
        {
            Event = Event ?? new Event {
                Startdate = DateTime.Now,
                Duration = DateTime.Now
            };
            /*Включение навигационных свойств*/
            var Model = ObjectViewModelFactory<Event>.Create(Event);
            EventConfigureViewBag();
            return View("EventEditor", Model);
        }

        public IActionResult CreateEvent() => LocalCreateEvent(null);

        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromForm] ObjectViewModel<Event> Event)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Event.Object.Description = Methods.CoalesceString(Event.Object.Description);
                    Event.Object.Canhavewinners = Event.Option[1];
                    Context.Event.Add(Event.Object);
                    await Context.SaveChangesAsync();
                    return RedirectToAction(nameof(Event));
                }
                catch (Exception exception)
                {
                    this.HandleException(exception);
                }
            }
            return LocalCreateEvent(Event.Object);
        }
        public IActionResult DeleteEvent(string id)
        {
            var Event = Context.Event
                /*Включение навигационных свойств*/
                .First(p => p.Id == int.Parse(id));
            return View("EventEditor", ObjectViewModelFactory<Event>.Delete(Event));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteEvent([FromForm] ObjectViewModel<Event> Event)
        {
            try
            {
                Context.Event.Remove(Event.Object);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(Event));
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
            }
            return View("EventEditor", ObjectViewModelFactory<Event>.Delete(Event.Object));

        }
        #endregion

        #region Winner
        private void WinnerConfigureViewBag(int eid)
        {
            ViewBag.Yacht = Context.Yacht
                .Include(p => p.Type)
                .Where(p => Context.Busyyacht.Any(y => p.Id == y.Id && !y.R.Value && !y.C.Value && !y.E.Value && y.Val.Value))
                .Where(y => y.YachtCrew.Any(yc => yc.Enddate == null && yc.Crew.Position.Name == "Captain"));
            ViewBag.Event = Context.Event.First(p => p.Id == eid);
        }

        private IActionResult LocalEditWinner(int eid, int yid, Winner Winner = null)
        {
            Winner = Winner ?? Context.Winner
               /*Включение навигационных свойств*/
               .First(p => p.Eventid == eid && p.Yachtid == yid);
            /*Включение навигационных свойств*/
            var Model = ObjectViewModelFactory<Winner>.Edit(Winner);
            WinnerConfigureViewBag(eid);
            return View("WinnerEditor", Model);
        }

        public IActionResult EditWinner(int eid, int yid) => LocalEditWinner(eid, yid);

        [HttpPost]
        public async Task<IActionResult> EditWinner([FromForm] ObjectViewModel<Winner> Winner)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Winner.Option[0])
                    {
                        /*Опции*/
                    }
                    var Wi = Context.Winner.First(w => w.Eventid == Winner.Object.Eventid && w.Yachtid == Winner.Object.Yachtid);
                    Wi.Eventid = Winner.Object.Eventid;
                    Wi.Yachtid = Winner.Object.Yachtid;
                    Wi.Place = Winner.Object.Place;
                    await Context.SaveChangesAsync();
                    return RedirectToAction(nameof(Event));
                }
                catch (Exception exception)
                {
                    this.HandleException(exception);
                }
            }

            return LocalEditWinner(Winner.Object.Eventid, Winner.Object.Yachtid, Winner.Object);
        }

        private IActionResult LocalCreateWinner(int eid, Winner Winner = null)
        {
            Winner = Winner ?? new Winner { 
                Eventid = eid
            };
            /*Включение навигационных свойств*/
            var Model = ObjectViewModelFactory<Winner>.Create(Winner);
            WinnerConfigureViewBag(eid);
            return View("WinnerEditor", Model);
        }

        public IActionResult CreateWinner(int eid) => LocalCreateWinner(eid);

        [HttpPost]
        public async Task<IActionResult> CreateWinner([FromForm] ObjectViewModel<Winner> Winner)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //Winner.Object.Description = Methods.CoalesceString(Winner.Object.Description);
                    Context.Winner.Add(Winner.Object);
                    await Context.SaveChangesAsync();
                    return RedirectToAction(nameof(Event));
                }
                catch (Exception exception)
                {
                    this.HandleException(exception);
                }
            }
            return LocalCreateWinner(Winner.Object.Eventid,Winner.Object);
        }
        public IActionResult DeleteWinner(int eid, int yid)
        {
            var Winner = Context.Winner
                 /*Включение навигационных свойств*/
                 .First(p => p.Eventid == eid && p.Yachtid == yid);
            return View("WinnerEditor", ObjectViewModelFactory<Winner>.Delete(Winner));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteWinner([FromForm] ObjectViewModel<Winner> Winner)
        {
            try
            {
                Context.Winner.Remove(Winner.Object);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(Event));
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
            }
            return View("WinnerEditor", ObjectViewModelFactory<Winner>.Delete(Winner.Object));

        }
        #endregion

        #region Contracttype
        public IActionResult Contracttype()
        {
            var Object = Context.Contracttype
                /*Включение навигационных свойств*/
                .OrderBy(p => p.Id);
            return View(Object);
        }

        private void ContracttypeConfigureViewBag()
        {
            //Для ViewBag
        }

        private IActionResult LocalEditContracttype(string id, Contracttype Contracttype = null)
        {
            Contracttype = Contracttype ?? Context.Contracttype
               /*Включение навигационных свойств*/
               .First(p => p.Id == int.Parse(id));
            /*Включение навигационных свойств*/
            var Model = ObjectViewModelFactory<Contracttype>.Edit(Contracttype);
            ContracttypeConfigureViewBag();
            return View("ContracttypeEditor", Model);
        }

        public IActionResult EditContracttype(string id) => LocalEditContracttype(id);

        [HttpPost]
        public async Task<IActionResult> EditContracttype([FromForm] ObjectViewModel<Contracttype> Contracttype)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (Contracttype.Option[0])
                    {
                        /*Опции*/
                    }
                    //Contracttype.Object.Description = Methods.CoalesceString(Contracttype.Object.Description);
                    Context.Contracttype.Update(Contracttype.Object);
                    await Context.SaveChangesAsync();
                    return RedirectToAction(nameof(Contracttype));
                }
                catch (Exception exception)
                {
                    this.HandleException(exception);
                }
            }

            return LocalEditContracttype($"{Contracttype.Object.Id}", Contracttype.Object);
        }

        private IActionResult LocalCreateContracttype(Contracttype Contracttype = null)
        {
            Contracttype = Contracttype ?? new Contracttype();
            /*Включение навигационных свойств*/
            var Model = ObjectViewModelFactory<Contracttype>.Create(Contracttype);
            ContracttypeConfigureViewBag();
            return View("ContracttypeEditor", Model);
        }

        public IActionResult CreateContracttype() => LocalCreateContracttype(null);

        [HttpPost]
        public async Task<IActionResult> CreateContracttype([FromForm] ObjectViewModel<Contracttype> Contracttype)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //Contracttype.Object.Description = Methods.CoalesceString(Contracttype.Object.Description);
                    Context.Contracttype.Add(Contracttype.Object);
                    await Context.SaveChangesAsync();
                    return RedirectToAction(nameof(Contracttype));
                }
                catch (Exception exception)
                {
                    this.HandleException(exception);
                }
            }
            return LocalCreateContracttype(Contracttype.Object);
        }
        public IActionResult DeleteContracttype(string id)
        {
            var Contracttype = Context.Contracttype
                /*Включение навигационных свойств*/
                .First(p => p.Id == int.Parse(id));
            return View("ContracttypeEditor", ObjectViewModelFactory<Contracttype>.Delete(Contracttype));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteContracttype([FromForm] ObjectViewModel<Contracttype> Contracttype)
        {
            try
            {
                Context.Contracttype.Remove(Contracttype.Object);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(Contracttype));
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
            }
            return View("ContracttypeEditor", ObjectViewModelFactory<Contracttype>.Delete(Contracttype.Object));

        }
        #endregion

        //NOTE: Везде проставить текстовые еденицы. Спросить

        #region Contract
        public IActionResult Contract()
        {
            var Object = Context.Contract
                /*Включение навигационных свойств*/
                .Include(p => p.Client)
                .Include(p => p.Contracttype)
                .Include(p => p.Captaininyacht)
                    .ThenInclude(p => p.Yacht)
                        .ThenInclude(p => p.Type)
                .Include(p => p.Captaininyacht)
                    .ThenInclude(p => p.Crew)
                        .ThenInclude(p => p.Staff)
                .OrderByDescending(p => p.Enddate ?? DateTime.Now)
                .ThenBy(p => p.Id);
            return View(Object);
        }

        private void ContractConfigureViewBag()
        {
            ViewBag.Captain = Context.YachtCrew
                .Include(p => p.Yacht)
                    .ThenInclude(p => p.Type)
                .Include(p => p.Crew)
                    .ThenInclude(p => p.Position)
                .Include(p => p.Crew)
                    .ThenInclude(p => p.Staff)
                .Where(p => p.Crew.Position.Name == "Captain")
                .Where(c => c.Enddate == null)
                .Where(c => Context.Busyyacht.Any(b => b.Id == c.Yachtid && !b.R.Value && !b.C.Value && !b.E.Value && b.Filled.Value && b.Val.Value))
                ;
            ViewBag.Client = Context.Person.Where(p => !p.Staffonly);
            ViewBag.Type = Context.Contracttype.ToList();
        }

        private IActionResult LocalEditContract(string id = null, Contract Contract = null)
        {
            Contract = Contract ?? Context.Contract
               /*Включение навигационных свойств*/
               .First(p => p.Id == int.Parse(id));
            /*Включение навигационных свойств*/
            var Model = ObjectViewModelFactory<Contract>.Edit(Contract);
            ContractConfigureViewBag();
            return View("ContractEditor", Model);
        }

        public IActionResult EditContract(string id) => LocalEditContract(id);
        [HttpPost]
        public async Task<IActionResult> EditContract([FromForm] ObjectViewModel<Contract> Contract)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var contract = Context.Contract.First(c => Contract.Object.Id == c.Id);
                    if (Contract.Option[0])
                    {
                        contract.Enddate = DateTime.Now;
                    }
                    if (Contract.Option[1])
                    {
                        contract.Averallprice = null;
                    }
                    contract.Duration = Contract.Object.Duration;
                    contract.Specials = Methods.CoalesceString(Contract.Object.Specials);
                    contract.Paid = Contract.Object.Paid;
                    await Context.SaveChangesAsync();
                    return RedirectToAction(nameof(Contract));
                }
                catch (Exception exception)
                {
                    this.HandleException(exception);
                }
            }

            return LocalEditContract(Contract: Contract.Object);
        }

        private IActionResult LocalCreateContract(Contract Contract = null)
        {
            Contract = Contract ?? new Contract {
                Startdate = DateTime.Now,
                Duration = DateTime.Now.AddDays(1),
                Averallprice = null
            };
            /*Включение навигационных свойств*/
            var Model = ObjectViewModelFactory<Contract>.Create(Contract);
            ContractConfigureViewBag();
            return View("ContractEditor", Model);
        }

        public IActionResult CreateContract() => LocalCreateContract(null);

        [HttpPost]
        public async Task<IActionResult> CreateContract([FromForm] ObjectViewModel<Contract> Contract)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //Contract.Object.Description = Methods.CoalesceString(Contract.Object.Description);
                    Context.Contract.Add(Contract.Object);
                    await Context.SaveChangesAsync();
                    return RedirectToAction(nameof(Contract));
                }
                catch (Exception exception)
                {
                    this.HandleException(exception);
                }
            }
            return LocalCreateContract(Contract.Object);
        }
        public IActionResult DeleteContract(string id)
        {
            var Contract = Context.Contract
                /*Включение навигационных свойств*/
                .First(p => p.Id == int.Parse(id));
            return View("ContractEditor", ObjectViewModelFactory<Contract>.Delete(Contract));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteContract([FromForm] ObjectViewModel<Contract> Contract)
        {
            try
            {
                Context.Contract.Remove(Contract.Object);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(Contract));
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
            }
            return View("ContractEditor", ObjectViewModelFactory<Contract>.Delete(Contract.Object));

        }
        #endregion


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
                try
                {
                    Context.Yachtleasetype.Update(Yachtleasetype.Object);
                    await Context.SaveChangesAsync();
                    return RedirectToAction(nameof(Yachtleasetype));
                }
                catch (Exception exception)
                {
                    this.HandleException(exception);
                }
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

                try
                {
                    Context.Yachtleasetype.Add(Yachtleasetype.Object);
                    await Context.SaveChangesAsync();
                    return RedirectToAction(nameof(Yachtleasetype));
                }
                catch (Exception exception)
                {
                    this.HandleException(exception);
                }
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
            try
            {
                Context.Yachtleasetype.Remove(Yachtleasetype.Object);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(Yachtleasetype));
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
            }
            return View("YachtleasetypeEditor", ObjectViewModelFactory<Yachtleasetype>.Delete(Yachtleasetype.Object));
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
                .OrderByDescending(p => p.Enddate ?? DateTime.Now);
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
                try
                {
                    Context.Yachttype.Update(staff.Object);
                    await Context.SaveChangesAsync();
                    return RedirectToAction(nameof(Yachttype));
                }
                catch (Exception exception)
                {
                    this.HandleException(exception);
                }


            }

            return View("YachttypeEditor", ObjectViewModelFactory<Yachttype>.Edit(staff.Object));
        }

        public IActionResult CreateYachttype()
        {
            var Yachttype = new Yachttype
            {
                Description = ""
            };
            return View("YachttypeEditor", ObjectViewModelFactory<Yachttype>.Create(Yachttype));
        }

        [HttpPost]
        public async Task<IActionResult> CreateYachttype([FromForm] ObjectViewModel<Yachttype> staff)
        {
            if (ModelState.IsValid)
            {
              
                try
                {
                    Context.Yachttype.Add(staff.Object);
                    await Context.SaveChangesAsync();
                    return RedirectToAction(nameof(Yachttype));
                }
                catch (Exception exception)
                {
                    this.HandleException(exception);
                }
            }

            return View("YachttypeEditor", ObjectViewModelFactory<Yachttype>.Create(staff.Object));
        }
        public IActionResult DeleteYachttype(string id)
        {
            var Yachttype = Context.Yachttype.First(p => p.Id == int.Parse(id));
            return View("YachttypeEditor", ObjectViewModelFactory<Yachttype>.Delete(Yachttype));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteYachttype([FromForm] ObjectViewModel<Yachttype> Yachttype)
        {
            try
            {
                Context.Yachttype.Remove(Yachttype.Object);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(Yachttype));
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
            }

            return View("YachttypeEditor", ObjectViewModelFactory<Yachttype>.Delete(Yachttype.Object));
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
            PositionYachttype = PositionYachttype ?? new PositionYachttype
            {
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
