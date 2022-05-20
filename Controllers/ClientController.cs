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


        #region Review
        public IActionResult Review()
        {
            var Object = Context.Review
                .Include(p => p.Client)
                .OrderBy(p => p.Id)
                .Select(p => new ReviewViewModel
                {
                    Review = p,
                    Captains = Context.ReviewCaptain.Include(p => p.Captain).Where(r => r.Reviewid == p.Id),
                    Yachts = Context.ReviewYacht.Include(p => p.Yacht).ThenInclude(p => p.Type).Where(r => r.Reviewid == p.Id)
                })
                ;
            return View(Object);
        }

        private void ReviewConfigureViewBag()
        {
            if (Methods.IsDev) ViewBag.Client = Context.Person;
        }

        private IActionResult LocalEditReview(string id, Review Review = null)
        {
            Review = Review ?? Context.Review
               /*Включение навигационных свойств*/
               .First(p => p.Id == int.Parse(id));
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
            Review = Review ?? new Review { 
                Rate = 1,
                Public = true,
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
                .First(p => p.Id == int.Parse(id));
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

        #region ReviewYacht
        private void ReviewYachtConfigureViewBag()
        {
            ViewBag.Other = Context.Yacht.Include(p => p.Type);
        }
        private IActionResult LocalCreateReviewYacht(int rid = 0, string ReturnUrl = null , ReviewYacht ReviewYacht = null)
        {
            ReviewYacht = ReviewYacht ?? new ReviewYacht { 
                 Reviewid = rid
            };
            /*Включение навигационных свойств*/
            var Model = ObjectViewModelFactory<ReviewYacht>.Create(ReviewYacht, ReturnUrl);
            ReviewYachtConfigureViewBag();
            return View("ReviewYachtEditor", Model);
        }

        public IActionResult CreateReviewYacht(int rid) => LocalCreateReviewYacht(rid);

        [HttpPost]
        public async Task<IActionResult> CreateReviewYacht([FromForm] ObjectViewModel<ReviewYacht> ReviewYacht)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //ReviewYacht.Object.Description = Methods.CoalesceString(ReviewYacht.Object.Description);
                    Context.ReviewYacht.Add(ReviewYacht.Object);
                    await Context.SaveChangesAsync();
                    return RedirectToAction(nameof(Review));
                }
                catch (Exception exception)
                {
                    this.HandleException(exception);
                }
            }
            return LocalCreateReviewYacht(ReviewYacht: ReviewYacht.Object);
        }
        public IActionResult DeleteReviewYacht(int rid, int cid, string ReturnUrl)
        {
            var ReviewYacht = Context.ReviewYacht
                /*Включение навигационных свойств*/
                .First(p => p.Reviewid == rid && p.Yachtid == cid);
            return View("ReviewYachtEditor", ObjectViewModelFactory<ReviewYacht>.Delete(ReviewYacht, ReturnUrl));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteReviewYacht([FromForm] ObjectViewModel<ReviewYacht> ReviewYacht)
        {
            try
            {
                Context.ReviewYacht.Remove(ReviewYacht.Object);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(Review));
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
            }
            return View("ReviewYachtEditor", ObjectViewModelFactory<ReviewYacht>.Delete(ReviewYacht.Object));

        }
        #endregion

        #region ReviewCaptain
        private void ReviewCaptainConfigureViewBag()
        {
            ViewBag.Other = Context.Person.Where(a => Context.YachtCrew.Include(p => p.Crew).Any(p => p.Enddate == null && p.Crew.Staffid == a.Id));
        }
        private IActionResult LocalCreateReviewCaptain(int rid = 0, string ReturnUrl = null,  ReviewCaptain ReviewCaptain = null)
        {
            ReviewCaptain = ReviewCaptain ?? new ReviewCaptain { 
                 Reviewid = rid
            };
            /*Включение навигационных свойств*/
            var Model = ObjectViewModelFactory<ReviewCaptain>.Create(ReviewCaptain, ReturnUrl);
            ReviewCaptainConfigureViewBag();
            return View("ReviewCaptainEditor", Model);
        }

        public IActionResult CreateReviewCaptain(int rid) => LocalCreateReviewCaptain(rid);

        [HttpPost]
        public async Task<IActionResult> CreateReviewCaptain([FromForm] ObjectViewModel<ReviewCaptain> ReviewCaptain)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Context.ReviewCaptain.Add(ReviewCaptain.Object);
                    await Context.SaveChangesAsync();
                    return RedirectToAction(nameof(Review));
                }
                catch (Exception exception)
                {
                    this.HandleException(exception);
                }
            }
            return LocalCreateReviewCaptain(ReviewCaptain: ReviewCaptain.Object);
        }
        public IActionResult DeleteReviewCaptain(int rid, int cid, string ReturnUrl)
        {
            var ReviewCaptain = Context.ReviewCaptain
                .First(p => p.Reviewid == rid && p.Captainid == cid);
            return View("ReviewCaptainEditor", ObjectViewModelFactory<ReviewCaptain>.Delete(ReviewCaptain, ReturnUrl));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteReviewCaptain([FromForm] ObjectViewModel<ReviewCaptain> ReviewCaptain)
        {
            try
            {
                Context.ReviewCaptain.Remove(ReviewCaptain.Object);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(Review));
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
            }
            return View("ReviewCaptainEditor", ObjectViewModelFactory<ReviewCaptain>.Delete(ReviewCaptain.Object));

        }
        #endregion

    }
}
