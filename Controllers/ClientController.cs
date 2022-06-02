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
    [Authorize]
    public class ClientController : Controller
    {
        public DataContext Context;
        public ClientController(DataContext context)
        {
            Context = context;
        }

        #region Yacht
        public IActionResult Yacht()
        {
            var Yacht = Context.Yacht
                .Include(p => p.Yachtowner)
                .Include(p => p.Type)
                .OrderBy( y => Context.Yachtlease.Where(p => p.Enddate == null && p.Yachtid == y.Id).Select(p => p.Startdate).First())
                .Where(p => p.Yachtownerid == User.PersonId())
                .Select(
                    p => new YachtWithStatusViewModel
                    {
                        Yacht = p,
                        Status = Context.YachtsStatus(p.Id),
                        Crew = Context.YachtCrew
                        .Include(p => p.Crew)
                            .ThenInclude(p => p.Staff)
                        .Include(p => p.Crew)
                            .ThenInclude(p => p.Position)
                        .Where(y => y.Enddate == null && y.Yachtid == p.Id)
                        .ToList(),                        
                    }
                )
                .ToList();
            Yacht.ForEach(p => {
                p.Tests = Context.YachttestInfo(p.Yacht.Id).ToList();
                p.Repairs = Context.RepairInfo(p.Yacht.Id).ToList();
            });

            return View(Yacht);
        }

        private void ConfigureVIewBagYacht()
        {
            ViewData["Yachtowner"] = Context.Person
               .Where(p => !p.Staffonly)
               .Where(p => Context.CountRoleByName(p.Email) > 0)
               .ToList();
        }

        
        public IActionResult EditYacht(string id)
        {
            var Yacht = Context.Yacht
                .Include(p => p.Yachtowner)
                .Include(p => p.Type)
                .First(p => p.Id == int.Parse(id));
            var Model = ObjectViewModelFactory<Yacht>.Edit(Yacht);
            ConfigureVIewBagYacht();
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
                    Yacht.Object.Description = Methods.CoalesceString(Yacht.Object.Description);
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
            ConfigureVIewBagYacht();
            return View("YachtEditor", Model);


        }

        public IActionResult CreateYacht()
        {
            var Yacht = new Yacht
            {
                Description = "",
                Rentable = false,
                Yachtownerid = User.PersonId()
            };
            var Model = ObjectViewModelFactory<Yacht>.Create(Yacht);
            ViewData["Type"] = Context.Yachttype;
            return View("YachtEditor", Model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateYacht([FromForm] ObjectViewModel<Yacht> Yacht)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Yacht.Object.Yachtownerid = User.PersonId();
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
            return View("YachtEditor", Model);
        }

        public IActionResult DeleteYacht(string id)
        {
            var Yacht = Context.Yacht
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

        public IActionResult Yachtlease()
        {
            var Yachtlease = Context.Yachtlease
                .Include(p => p.Yacht)
                    .ThenInclude(p => p.Type)
                .Include(p => p.Yachtleasetype)
                .Where(p => p.Yacht.Yachtownerid == User.PersonId())
                .OrderByDescending(p => p.Enddate ?? DateTime.Now)
                .ThenBy(p => p.Paid);
            return View("Yachtlease", Yachtlease);
        }

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
               .Where(p => p.Clientid == User.PersonId())
               .OrderByDescending(p => p.Enddate ?? DateTime.Now)
               .ThenBy(p => p.Id)
              ;

            return View("Contract", Object);
        }

        #region Review
        public IActionResult Review()
        {
            if (User.IsInRole(RolesReadonly.Client))
            {
                var Object = Context.Review
                    .Include(p => p.Client)
                    .Where(p => p.Clientid == User.PersonId())
                    .OrderByDescending(p => p.Date)
                    .Select(p => new ReviewViewModel
                    {
                        Review = p
                    })
                    ;
                return View(Object);
            }
            else
            {
                var Object = Context.Review
                                   .Include(p => p.Client)
                                   .OrderByDescending(p => p.Date)
                                   .Select(p => new ReviewViewModel
                                   {
                                       Review = p
                                   })
                                   ;
                return View(Object);
            }
        }

        private void ReviewConfigureViewBag()
        {
        }

        private IActionResult LocalEditReview(string id, Review Review = null)
        {
            Review = Review ?? Context.Review
               /*Включение навигационных свойств*/
               .First(p => p.Id == int.Parse(id) && p.Clientid == User.PersonId());
            /*Включение навигационных свойств*/
            var Model = ObjectViewModelFactory<Review>.Edit(Review);
            ReviewConfigureViewBag();
            return View("ReviewEditor", Model);
        }

        public IActionResult EditReview(string id) => LocalEditReview(id);

        [HttpPost]
        public async Task<IActionResult> EditReview([FromForm] ObjectViewModel<Review> Review)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Review.Object.Text = Methods.CoalesceString(Review.Object.Text);
                    Context.Review.Update(Review.Object);
                    await Context.SaveChangesAsync();
                    return RedirectToAction(nameof(Review));
                }
                catch (Exception exception)
                {
                    this.HandleException(exception);
                }
            }

            return LocalEditReview($"{Review.Object.Id}", Review.Object);
        }

        private IActionResult LocalCreateReview(Review Review = null)
        {
            Review = Review ?? new Review
            {
                Rate = 1,
                Public = true,
                Clientid = User.PersonId(),
                Date = DateTime.Now
            };
            /*Включение навигационных свойств*/
            var Model = ObjectViewModelFactory<Review>.Create(Review);
            ReviewConfigureViewBag();
            return View("ReviewEditor", Model);
        }

        public IActionResult CreateReview() => LocalCreateReview(null);

        [HttpPost]
        public async Task<IActionResult> CreateReview([FromForm] ObjectViewModel<Review> Review)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Review.Object.Text = Methods.CoalesceString(Review.Object.Text);
                    Context.Review.Add(Review.Object);
                    await Context.SaveChangesAsync();
                    return RedirectToAction(nameof(Review));
                }
                catch (Exception exception)
                {
                    this.HandleException(exception);
                }
            }
            return LocalCreateReview(Review.Object);
        }
        public IActionResult DeleteReview(string id)
        {
            var Review = Context.Review
                /*Включение навигационных свойств*/
                .First(p => p.Id == int.Parse(id) && p.Clientid == User.PersonId());
            return View("ReviewEditor", ObjectViewModelFactory<Review>.Delete(Review));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteReview([FromForm] ObjectViewModel<Review> Review)
        {
            try
            {
                Context.Review.Remove(Review.Object);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(Review));
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
            }
            return View("ReviewEditor", ObjectViewModelFactory<Review>.Delete(Review.Object));

        }
        #endregion

        //#region ReviewYacht
        //private void ReviewYachtConfigureViewBag()
        //{
        //    ViewBag.Other = Context.Yacht.Include(p => p.Type);
        //}
        //private IActionResult LocalCreateReviewYacht(int rid = 0, string ReturnUrl = null, ReviewYacht ReviewYacht = null)
        //{
        //    ReviewYacht = ReviewYacht ?? new ReviewYacht
        //    {
        //        Reviewid = rid
        //    };
        //    /*Включение навигационных свойств*/
        //    var Model = ObjectViewModelFactory<ReviewYacht>.Create(ReviewYacht, ReturnUrl);
        //    ReviewYachtConfigureViewBag();
        //    return View("ReviewYachtEditor", Model);
        //}

        //public IActionResult CreateReviewYacht(int rid) => LocalCreateReviewYacht(rid);

        //[HttpPost]
        //public async Task<IActionResult> CreateReviewYacht([FromForm] ObjectViewModel<ReviewYacht> ReviewYacht)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            //ReviewYacht.Object.Description = Methods.CoalesceString(ReviewYacht.Object.Description);
        //            Context.ReviewYacht.Add(ReviewYacht.Object);
        //            await Context.SaveChangesAsync();
        //            return RedirectToAction(nameof(Review));
        //        }
        //        catch (Exception exception)
        //        {
        //            this.HandleException(exception);
        //        }
        //    }
        //    return LocalCreateReviewYacht(ReviewYacht: ReviewYacht.Object);
        //}
        //public IActionResult DeleteReviewYacht(int rid, int cid, string ReturnUrl)
        //{
        //    var ReviewYacht = Context.ReviewYacht
        //        /*Включение навигационных свойств*/
        //        .First(p => p.Reviewid == rid && p.Yachtid == cid);
        //    return View("ReviewYachtEditor", ObjectViewModelFactory<ReviewYacht>.Delete(ReviewYacht, ReturnUrl));
        //}

        //[HttpPost]
        //public async Task<IActionResult> DeleteReviewYacht([FromForm] ObjectViewModel<ReviewYacht> ReviewYacht)
        //{
        //    try
        //    {
        //        Context.ReviewYacht.Remove(ReviewYacht.Object);
        //        await Context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Review));
        //    }
        //    catch (Exception exception)
        //    {
        //        this.HandleException(exception);
        //    }
        //    return View("ReviewYachtEditor", ObjectViewModelFactory<ReviewYacht>.Delete(ReviewYacht.Object));

        //}
        //#endregion

        //#region ReviewCaptain
        //private void ReviewCaptainConfigureViewBag()
        //{
        //    ViewBag.Other = Context.Person.Where(a => Context.YachtCrew.Include(p => p.Crew).Any(p => p.Enddate == null && p.Crew.Staffid == a.Id));
        //}
        //private IActionResult LocalCreateReviewCaptain(int rid = 0, string ReturnUrl = null, ReviewCaptain ReviewCaptain = null)
        //{
        //    ReviewCaptain = ReviewCaptain ?? new ReviewCaptain
        //    {
        //        Reviewid = rid
        //    };
        //    /*Включение навигационных свойств*/
        //    var Model = ObjectViewModelFactory<ReviewCaptain>.Create(ReviewCaptain, ReturnUrl);
        //    ReviewCaptainConfigureViewBag();
        //    return View("ReviewCaptainEditor", Model);
        //}

        //public IActionResult CreateReviewCaptain(int rid) => LocalCreateReviewCaptain(rid);

        //[HttpPost]
        //public async Task<IActionResult> CreateReviewCaptain([FromForm] ObjectViewModel<ReviewCaptain> ReviewCaptain)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            Context.ReviewCaptain.Add(ReviewCaptain.Object);
        //            await Context.SaveChangesAsync();
        //            return RedirectToAction(nameof(Review));
        //        }
        //        catch (Exception exception)
        //        {
        //            this.HandleException(exception);
        //        }
        //    }
        //    return LocalCreateReviewCaptain(ReviewCaptain: ReviewCaptain.Object);
        //}
        //public IActionResult DeleteReviewCaptain(int rid, int cid, string ReturnUrl)
        //{
        //    var ReviewCaptain = Context.ReviewCaptain
        //        .First(p => p.Reviewid == rid && p.Captainid == cid);
        //    return View("ReviewCaptainEditor", ObjectViewModelFactory<ReviewCaptain>.Delete(ReviewCaptain, ReturnUrl));
        //}

        //[HttpPost]
        //public async Task<IActionResult> DeleteReviewCaptain([FromForm] ObjectViewModel<ReviewCaptain> ReviewCaptain)
        //{
        //    try
        //    {
        //        Context.ReviewCaptain.Remove(ReviewCaptain.Object);
        //        await Context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Review));
        //    }
        //    catch (Exception exception)
        //    {
        //        this.HandleException(exception);
        //    }
        //    return View("ReviewCaptainEditor", ObjectViewModelFactory<ReviewCaptain>.Delete(ReviewCaptain.Object));

        //}
        //#endregion

    }
}
