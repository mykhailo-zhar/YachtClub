using Microsoft.AspNetCore.Mvc;
using Project.Database;
using Project.Migrations;
using Project.Models;
using System.Linq;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Project.Controllers
{
    
    public class CaptainController : Controller
    {
        private DataContext Context;
        public CaptainController(DataContext dataContext)
        {
            Context = dataContext;
        }

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
                .Where(p => p.Captaininyacht.Crew.Staffid == User.PersonId())
                .OrderByDescending(p => p.Enddate ?? DateTime.Now)
                .ThenBy(p => p.Id)
               ;
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
                .Where(p => p.Crew.Staffid == User.PersonId())
                .Where(c => Context.Busyyacht.Any(b => b.Id == c.Yachtid && !b.R && !b.C && !b.E && b.Filled && b.Val))
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
                        if(contract.Startdate > contract.Enddate) throw new Exception("Дата окончания контракта раньше чем дата начала контракта");
                    }
                    if (Contract.Option[1])
                    {
                        contract.Averallprice = null;
                    }
                    if (!Contract.Object.Paid)
                    {
                        contract.Startdate = Contract.Object.Startdate;
                    }
                    else if(contract.Startdate != Contract.Object.Startdate)
                    {
                        throw new Exception("Нельзя поменять дату начала контракта после оплаты");
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
            Contract = Contract ?? new Contract
            {
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
    }
}
