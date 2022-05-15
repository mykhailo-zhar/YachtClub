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
                try
                {
                    Context.Materialtype.Update(Materialtype.Object);
                    await Context.SaveChangesAsync();
                    return RedirectToAction(nameof(Materialtype));
                }
                catch (Exception exception)
                {
                    this.HandleException(exception);
                }
                if (ModelState.IsValid)
                {
                    return RedirectToAction(nameof(Materialtype));
                }
                return View("MaterialtypeEditor", ObjectViewModelFactory<Materialtype>.Edit(Materialtype.Object));
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
                try
                {
                    Materialtype.Object.Metric = string.IsNullOrEmpty(Materialtype.Object.Metric) ? null : Materialtype.Object.Metric;
                    Context.Materialtype.Add(Materialtype.Object);
                    await Context.SaveChangesAsync();
                    return RedirectToAction(nameof(Materialtype));
                }
                catch (Exception exception)
                {
                    this.HandleException(exception);
                }
                if (ModelState.IsValid)
                {
                    return RedirectToAction(nameof(Materialtype));
                }
                return View("MaterialtypeEditor", ObjectViewModelFactory<Materialtype>.Create(Materialtype.Object));
            }
            return View("MaterialtypeEditor", ObjectViewModelFactory<Materialtype>.Create(Materialtype.Object));
        }

        #endregion
        
        #region Seller
        public IActionResult Seller()
        {
            var type = Context.Seller.OrderBy(p => p.Id);
            return View(type);
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
                try
                {
                    Context.Seller.Update(Seller.Object);
                    await Context.SaveChangesAsync();
                    return RedirectToAction(nameof(Seller));
                }
                catch (Exception exception)
                {
                    this.HandleException(exception);
                }
                if (ModelState.IsValid)
                {
                    return RedirectToAction(nameof(Materialtype));
                }
                return View("SellerEditor", ObjectViewModelFactory<Seller>.Edit(Seller.Object));
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
                try
                {
                    Context.Seller.Add(Seller.Object);
                    await Context.SaveChangesAsync();
                    return RedirectToAction(nameof(Seller));
                }
                catch (Exception exception)
                {
                    this.HandleException(exception);
                }
                if (ModelState.IsValid)
                {
                    return RedirectToAction(nameof(Materialtype));
                }
                return View("SellerEditor", ObjectViewModelFactory<Seller>.Create(Seller.Object));
            }
            return View("SellerEditor", ObjectViewModelFactory<Seller>.Create(Seller.Object));
        }
        #endregion

        #region Material
        public IActionResult Material()
        {
            var type = Context.Material
                .Include(p => p.Type)
                .OrderBy(p => p.Id).ToList();
            var res = Context.Availableresources.ToList();
            return View(new MaterialViewModel { Availableresources = res, Materials = type });
        }

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
                try
                {
                    Context.Material.Update(Material.Object);
                    await Context.SaveChangesAsync();
                    return RedirectToAction(nameof(Material));
                }
                catch (Exception exception)
                {
                    this.HandleException(exception);
                }
                if (ModelState.IsValid)
                {
                    return RedirectToAction(nameof(Materialtype));
                }
                return View("MaterialEditor", ObjectViewModelFactory<Material>.Edit(Material.Object));
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
                try
                {
                    Context.Material.Add(Material.Object);
                    await Context.SaveChangesAsync();
                    return RedirectToAction(nameof(Material));
                }
                catch (Exception exception)
                {
                    this.HandleException(exception);
                }
                if (ModelState.IsValid)
                {
                    return RedirectToAction(nameof(Materialtype));
                }
                return View("MaterialEditor", ObjectViewModelFactory<Material>.Create(Material.Object));
            }

            return View("MaterialEditor", ObjectViewModelFactory<Material>.Create(Material.Object));
        }

        #endregion

        #region Materiallease
        public IActionResult Materiallease()
        {
            var type = Context.Materiallease
                .Include(p => p.MaterialNavigation)
                    .ThenInclude(p => p.Type)
                .Include(p => p.SellerNavigation)
                .OrderBy(p => p.Id).ToList();
            
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
                try
                {
                    if (Materiallease.Option[0]) Materiallease.Object.Deliverydate = DateTime.Now;
                    Materiallease.Object.Overallprice = 0;
                    Context.Materiallease.Update(Materiallease.Object);
                    await Context.SaveChangesAsync();
                    return RedirectToAction(nameof(Materiallease));
                }
                catch (Exception exception)
                {
                    this.HandleException(exception);
                }
                if (ModelState.IsValid)
                {
                    return RedirectToAction(nameof(Materiallease));
                }

                return View("MaterialleaseEditor", ObjectViewModelFactory<Materiallease>.Edit(Materiallease.Object));
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
                try
                {
                    Materiallease.Object.Startdate = DateTime.Now;
                    Materiallease.Object.Deliverydate = null;
                    Materiallease.Object.Overallprice = 0;
                    Context.Materiallease.Add(Materiallease.Object);
                    await Context.SaveChangesAsync();
                    return RedirectToAction(nameof(Materiallease));
                }
                catch (Exception exception)
                {
                    this.HandleException(exception);
                }
                if (ModelState.IsValid)
                {
                    return RedirectToAction(nameof(Materiallease));
                }
                return View("MaterialleaseEditor", ObjectViewModelFactory<Materiallease>.Create(Materiallease.Object));
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
            try
            {
                Context.Materiallease.Remove(Materiallease.Object);
                await Context.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
            }
            if (ModelState.IsValid)
            {
                return RedirectToAction(nameof(Materiallease));
            }
            return View("MaterialleaseEditor", ObjectViewModelFactory<Materiallease>.Delete(Materiallease.Object));
        }

        #endregion


    }
}
