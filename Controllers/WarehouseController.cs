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
    [Authorize(Policy = "Warehouse")]
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
            }
            return View("MaterialtypeEditor", ObjectViewModelFactory<Materialtype>.Create(Materialtype.Object));
        }

        public IActionResult DeleteMaterialtype(string id)
        {
            var Materialtype = Context.Materialtype
                /*Включение навигационных свойств*/
                .First(p => p.Id == int.Parse(id));
            return View("MaterialtypeEditor", ObjectViewModelFactory<Materialtype>.Delete(Materialtype));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMaterialtype([FromForm] ObjectViewModel<Materialtype> Materialtype)
        {
            try
            {
                Context.Materialtype.Remove(Materialtype.Object);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(Materialtype));
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
            }
            return View("MaterialtypeEditor", ObjectViewModelFactory<Materialtype>.Delete(Materialtype.Object));

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
            }
            return View("SellerEditor", ObjectViewModelFactory<Seller>.Create(Seller.Object));
        }

        public IActionResult DeleteSeller(string id)
        {
            var Seller = Context.Seller
                /*Включение навигационных свойств*/
                .First(p => p.Id == int.Parse(id));
            return View("SellerEditor", ObjectViewModelFactory<Seller>.Delete(Seller));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSeller([FromForm] ObjectViewModel<Seller> Seller)
        {
            try
            {
                Context.Seller.Remove(Seller.Object);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(Seller));
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
            }
            return View("SellerEditor", ObjectViewModelFactory<Seller>.Delete(Seller.Object));

        }

        #endregion

        #region Material
        public IActionResult Material()
        {
            var type = Context.Material
                .Include(p => p.Type)
                .OrderBy(p => p.Id)
                .Join(
                    Context.Availableresources,
                    p => p.Id,
                    a => a.Material,
                    (p, a) => new MaterialViewModel
                    {
                        Material = p,
                        Count = a.Count,
                        Format = a.Format
                    }
                )
                .ToList();
            return View(type);
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
            }

            return View("MaterialEditor", ObjectViewModelFactory<Material>.Create(Material.Object));
        }

        public IActionResult DeleteMaterial(string id)
        {
            var Material = Context.Material
                /*Включение навигационных свойств*/
                .First(p => p.Id == int.Parse(id));
            return View("MaterialEditor", ObjectViewModelFactory<Material>.Delete(Material));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMaterial([FromForm] ObjectViewModel<Material> Material)
        {
            try
            {
                Context.Material.Remove(Material.Object);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(Material));
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
            }
            return View("MaterialEditor", ObjectViewModelFactory<Material>.Delete(Material.Object));

        }

        #endregion

        #region Materiallease
        public IActionResult Materiallease()
        {
            var type = Context.Materiallease
                   .Include(p => p.MaterialNavigation)
                       .ThenInclude(p => p.Type)
                   .Include(p => p.SellerNavigation)
                   .OrderByDescending(p => p.Deliverydate ?? DateTime.Now)
                   .Join(
                       Context.Availableresources,
                       p => p.Material,
                       a => a.Material,
                       (p, a) => new MaterialLeaseWithMetricsViewModel
                       {
                           Materiallease = p,
                           Count = a.Count,
                           Format = a.Format
                       }
                   )
                   .ToList();

            return View(type);
        }

        private void MaterialleaseConfigureViewBag(string id)
        {
            ViewData["Material"] = Context.Material.Include(p => p.Type).ToList();
            ViewData["Seller"] = Context.Seller.ToList();
            ViewBag.Metric = Context.Availableresources.FirstOrDefault(p => p.Material == int.Parse(id))?.Format;
        }

        private IActionResult LocalEditMateriallease(string id, Materiallease Materiallease = null)
        {
            Materiallease = Materiallease ?? Context.Materiallease
                .Include(p => p.MaterialNavigation)
                    .ThenInclude(p => p.Type)
                .First(p => p.Id == int.Parse(id));
            /*Включение навигационных свойств*/
            var Model = ObjectViewModelFactory<Materiallease>.Edit(Materiallease);
            MaterialleaseConfigureViewBag(id);
            return View("MaterialleaseEditor", Model);
        }

        public IActionResult EditMateriallease(string id) => LocalEditMateriallease(id);

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
            }

            return LocalEditMateriallease($"{Materiallease.Object.Id}", Materiallease.Object);
        }

        private IActionResult LocalCreateMateriallease(Materiallease Materiallease = null)
        {
            Materiallease = Materiallease ?? new Materiallease
            {

            };
            /*Включение навигационных свойств*/
            var Model = ObjectViewModelFactory<Materiallease>.Create(Materiallease);
            MaterialleaseConfigureViewBag("0");
            return View("MaterialleaseEditor", Model);
        }

        public IActionResult CreateMateriallease() => LocalCreateMateriallease(null);

        [HttpPost]
        public async Task<IActionResult> CreateMateriallease([FromForm] ObjectViewModel<Materiallease> Materiallease)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Materiallease.Object.Startdate = DateTime.Now;
                    Materiallease.Object.Deliverydate = null;
                    Context.Materiallease.Add(Materiallease.Object);
                    await Context.SaveChangesAsync();
                    return RedirectToAction(nameof(Materiallease));
                }
                catch (Exception exception)
                {
                    this.HandleException(exception);
                }
            }
            return LocalCreateMateriallease(Materiallease.Object);
        }
        public IActionResult DeleteMateriallease(string id)
        {
            var Materiallease = Context.Materiallease
                /*Включение навигационных свойств*/
                .First(p => p.Id == int.Parse(id));
            return View("MaterialleaseEditor", ObjectViewModelFactory<Materiallease>.Delete(Materiallease));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMateriallease([FromForm] ObjectViewModel<Materiallease> Materiallease)
        {
            try
            {
                Context.Materiallease.Remove(Materiallease.Object);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(Materiallease));
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
            }
            return View("MaterialleaseEditor", ObjectViewModelFactory<Materiallease>.Delete(Materiallease.Object));

        }
        #endregion


        [HttpGet]
        public IActionResult MaterialAnalytics(MaterialInfo Object)
        {
            Object = Object.LikeName == null ? new MaterialInfo
            {
                Flag = true,
                LikeName = "",
                LikeTypeName = "",
                From = DateTime.Today,
                To = DateTime.Today
            }
            : Object
            ;

            Object.Materials = Context.MaterialAnalytics(Object.LikeName,
                Object.LikeTypeName,
                Object.From,
                Object.To).OrderBy(p => p.Name)
                .ToList();
            return View(Object);
        }

    }
}
