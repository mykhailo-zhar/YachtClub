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
    public class WarehouseController : Controller
    {
        public DataContext Context;
        public WarehouseController(DataContext context)
        {
            Context = context;
        }

        #region Materialtype
        public IActionResult Materialtype()
        {
            var type = Context.Materialtype.OrderBy(p => p.Id);
            return View(type);
        }

        public IActionResult DetailsMaterialtype(string id)
        {
            var Materialtype = Context.Materialtype.First(p => p.Id == int.Parse(id));
            return View("_Details", new DetailsViewModel
            {
                Description = Materialtype.Description,
                ButtonsViewModel = new EditorBottomButtonsViewModel { 
                    BackAction = typeof(Materialtype).Name
                }
            }) ;
        }

        public IActionResult EditMaterialtype(string id)
        {
            var Materialtype = Context.Materialtype.First(p => p.Id == int.Parse(id));
            return View("MaterialtypeEditor", ObjectViewModelFactory<Materialtype>.Edit(Materialtype));
        }

        [HttpPost]
        public async Task<IActionResult> EditMaterialtype([FromForm] ObjectViewModel<Materialtype> Materialtype)
        {
            if (ModelState.IsValid)
            {
                Context.Materialtype.Update(Materialtype.Object);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(Materialtype));
            }

            return View("MaterialtypeEditor", ObjectViewModelFactory<Materialtype>.Edit(Materialtype.Object));
        }

        public IActionResult CreateMaterialtype()
        {
            var Materialtype = new Materialtype();
            return View("MaterialtypeEditor", ObjectViewModelFactory<Materialtype>.Create(Materialtype));
        }

        [HttpPost]
        public async Task<IActionResult> CreateMaterialtype([FromForm] ObjectViewModel<Materialtype> Materialtype)
        {
            if (ModelState.IsValid)
            {
                Materialtype.Object.Metric = string.IsNullOrEmpty(Materialtype.Object.Metric) ? null : Materialtype.Object.Metric;
                Context.Materialtype.Add(Materialtype.Object);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(Materialtype));
            }

            return View("MaterialtypeEditor", ObjectViewModelFactory<Materialtype>.Create(Materialtype.Object));
        }
        public IActionResult DeleteMaterialtype(string id)
        {
            var Materialtype = Context.Materialtype.First(p => p.Id == int.Parse(id));
            return View("MaterialtypeEditor", ObjectViewModelFactory<Materialtype>.Delete(Materialtype));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMaterialtype([FromForm] ObjectViewModel<Materialtype> Materialtype)
        {

            Context.Materialtype.Remove(Materialtype.Object);
            await Context.SaveChangesAsync();
            return RedirectToAction(nameof(Materialtype));

        }

        #endregion
        
        #region Seller
        public IActionResult Seller()
        {
            var type = Context.Seller.OrderBy(p => p.Id);
            return View(type);
        }

        public IActionResult DetailsSeller(string id)
        {
            var Seller = Context.Seller.First(p => p.Id == int.Parse(id));
            return View("_Details", new DetailsViewModel
            {
                Description = Seller.Description,
                ButtonsViewModel = new EditorBottomButtonsViewModel { 
                    BackAction = typeof(Seller).Name
                }
            }) ;
        }

        public IActionResult EditSeller(string id)
        {
            var Seller = Context.Seller.First(p => p.Id == int.Parse(id));
            return View("SellerEditor", ObjectViewModelFactory<Seller>.Edit(Seller));
        }

        [HttpPost]
        public async Task<IActionResult> EditSeller([FromForm] ObjectViewModel<Seller> Seller)
        {
            if (ModelState.IsValid)
            {
                Context.Seller.Update(Seller.Object);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(Seller));
            }

            return View("SellerEditor", ObjectViewModelFactory<Seller>.Edit(Seller.Object));
        }

        public IActionResult CreateSeller()
        {
            var Seller = new Seller();
            return View("SellerEditor", ObjectViewModelFactory<Seller>.Create(Seller));
        }

        [HttpPost]
        public async Task<IActionResult> CreateSeller([FromForm] ObjectViewModel<Seller> Seller)
        {
            if (ModelState.IsValid)
            {
                Context.Seller.Add(Seller.Object);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(Seller));
            }

            return View("SellerEditor", ObjectViewModelFactory<Seller>.Create(Seller.Object));
        }
        public IActionResult DeleteSeller(string id)
        {
            var Seller = Context.Seller.First(p => p.Id == int.Parse(id));
            return View("SellerEditor", ObjectViewModelFactory<Seller>.Delete(Seller));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSeller([FromForm] ObjectViewModel<Seller> Seller)
        {

            Context.Seller.Remove(Seller.Object);
            await Context.SaveChangesAsync();
            return RedirectToAction(nameof(Seller));

        }

        #endregion

        #region Material
        public IActionResult Material()
        {
            var type = Context.Material
                .Include(p => p.Type)
                .OrderBy(p => p.Id);
            return View(type);
        }

        //public IActionResult DetailsMaterial(string id)
        //{
        //    var Material = Context.Material.First(p => p.Id == int.Parse(id));
        //    return View("_Details", new DetailsViewModel
        //    {
        //        Description = Material.Description,
        //        ButtonsViewModel = new EditorBottomButtonsViewModel { 
        //            BackAction = typeof(Material).Name
        //        }
        //    }) ;
        //}

        public IActionResult EditMaterial(string id)
        {
            var Material = Context.Material.First(p => p.Id == int.Parse(id));
            ViewData["Materialtype"] = Context.Materialtype.ToList();
            return View("MaterialEditor", ObjectViewModelFactory<Material>.Edit(Material));
        }

        [HttpPost]
        public async Task<IActionResult> EditMaterial([FromForm] ObjectViewModel<Material> Material)
        {
            if (ModelState.IsValid)
            {
                Context.Material.Update(Material.Object);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(Material));
            }

            return View("MaterialEditor", ObjectViewModelFactory<Material>.Edit(Material.Object));
        }

        public IActionResult CreateMaterial()
        {
            var Material = new Material();
            ViewData["Materialtype"] = Context.Materialtype.ToList();
            return View("MaterialEditor", ObjectViewModelFactory<Material>.Create(Material));
        }

        [HttpPost]
        public async Task<IActionResult> CreateMaterial([FromForm] ObjectViewModel<Material> Material)
        {
            if (ModelState.IsValid)
            {
                Context.Material.Add(Material.Object);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(Material));
            }

            return View("MaterialEditor", ObjectViewModelFactory<Material>.Create(Material.Object));
        }
        public IActionResult DeleteMaterial(string id)
        {
            var Material = Context.Material.First(p => p.Id == int.Parse(id));
            ViewData["Materialtype"] = null;
            return View("MaterialEditor", ObjectViewModelFactory<Material>.Delete(Material));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMaterial([FromForm] ObjectViewModel<Material> Material)
        {

            Context.Material.Remove(Material.Object);
            await Context.SaveChangesAsync();
            return RedirectToAction(nameof(Material));

        }

        #endregion

        #region Materiallease
        public IActionResult Materiallease()
        {
            var type = Context.Materiallease
                .Include(p => p.MaterialNavigation)
                    .ThenInclude(p => p.Type)
                .Include(p => p.SellerNavigation)
                .OrderBy(p => p.Id);
            return View(type);
        }

        public IActionResult EditMateriallease(string id)
        {
            var Materiallease = Context.Materiallease
                .Include(p => p.MaterialNavigation)
                    .ThenInclude(p => p.Type)
                .First(p => p.Id == int.Parse(id));
            ViewData["Material"] = Context.Material.Include(p => p.Type).ToList();
            ViewData["Seller"] = Context.Seller.ToList();
            return View("MaterialleaseEditor", ObjectViewModelFactory<Materiallease>.Edit(Materiallease));
        }

        [HttpPost]
        public async Task<IActionResult> EditMateriallease([FromForm] ObjectViewModel<Materiallease> Materiallease)
        {
            if (ModelState.IsValid)
            {
                if (Materiallease.Option[0]) Materiallease.Object.Deliverydate = DateTime.Now;
                Materiallease.Object.Overallprice = Materiallease.Object.Count * Materiallease.Object.Priceperunit;
                Context.Materiallease.Update(Materiallease.Object);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(Materiallease));
            }

            return View("MaterialleaseEditor", ObjectViewModelFactory<Materiallease>.Edit(Materiallease.Object));
        }

        public IActionResult CreateMateriallease()
        {
            var Materiallease = new Materiallease();
            ViewData["Material"] = Context.Material.Include(p => p.Type).ToList();
            ViewData["Seller"] = Context.Seller.ToList();
            return View("MaterialleaseEditor", ObjectViewModelFactory<Materiallease>.Create(Materiallease));
        }

        [HttpPost]
        public async Task<IActionResult> CreateMateriallease([FromForm] ObjectViewModel<Materiallease> Materiallease)
        {
            if (ModelState.IsValid)
            {
                Materiallease.Object.Startdate = DateTime.Now;
                Materiallease.Object.Deliverydate = null;
                Materiallease.Object.Overallprice = Materiallease.Object.Count * Materiallease.Object.Priceperunit;
                Context.Materiallease.Add(Materiallease.Object);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(Materiallease));
            }

            return View("MaterialleaseEditor", ObjectViewModelFactory<Materiallease>.Create(Materiallease.Object));
        }
        public IActionResult DeleteMateriallease(string id)
        {
            var Materiallease = Context.Materiallease.First(p => p.Id == int.Parse(id));
            ViewData["Materialleasetype"] = null;
            return View("MaterialleaseEditor", ObjectViewModelFactory<Materiallease>.Delete(Materiallease));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMateriallease([FromForm] ObjectViewModel<Materiallease> Materiallease)
        {

            Context.Materiallease.Remove(Materiallease.Object);
            await Context.SaveChangesAsync();
            return RedirectToAction(nameof(Materiallease));

        }

        #endregion


    }
}
