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
    public class AccountController : Controller
    {
        private DataContext Context;
        public AccountController(DataContext dataContext)
        {
            Context = dataContext;
        }
        #region Login

        private IActionResult PrivateLogin(AccountViewModel account = null)
        {
            return View(
                new ObjectViewModel<AccountViewModel>
                {
                    Object = account ?? new AccountViewModel { 
                        User = new Person()
                    },
                    Action = "Login"
                }
                );
        }
        public IActionResult Login()
        {
            return PrivateLogin();
        }

        private IActionResult LoginToRolePrivate(ObjectViewModel<AccountViewModel> Account)
        {
            Account.Action = nameof(LoginToRole);
            return View("LoginToRole", Account);
        }
        [HttpPost]
        public async Task<IActionResult> LoginToRole(ObjectViewModel<AccountViewModel> Account)
        {
            Account.Object.AsWho = "Staff";
            return await Login(Account);
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromForm] ObjectViewModel<AccountViewModel> Account)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    switch (Account.Object.AsWho)
                    {
                        case "Staff":
                            break;
                        case "Client": break;
                        default:
                            throw new Exception("Войдите, пожалуйста, либо как сотрудник, либо как клиент");
                    }

                    if (string.IsNullOrEmpty(Account.Object.Password))
                    {
                        throw new Exception("Некорректные логин и(или) пароль");
                    }

                    var login = Account.Object.Login;
                    var position = Account.Object.Position;
                    var passwd = Account.Object.Password;

                    Account.Object.User = Context.Person.FirstOrDefault(p => p.Email == login);

                    try
                    {
                        if (Account.Object.User == null) throw new Exception();
                        Context.Ping(login, position ?? "Client", passwd);
                    }
                    catch (Exception)
                    {

                        throw new Exception("Некорректные логин и(или) пароль");
                    }

                    if (Account.Object.Position == null && Account.Object.AsWho == "Staff")
                    {
                        var collection = Context.StaffPosition
                            .Include(p => p.Position)
                            .Where(p => p.Staffid == Account.Object.User.Id && p.Enddate == null && !p.Position.DeclineAccess)
                            .Select(p => p.Position.Name)
                            .ToList();
                        if (collection.Count() == 0)
                        {
                            throw new Exception("Вы не можете войти как сотрудник");
                        }

                        ViewBag.Positions = collection;

                        return LoginToRolePrivate(Account);
                    } 

                    if (Account.Object.User.Staffonly && Account.Object.AsWho == "Client")
                    {
                        throw new Exception("Человек с служебным аккаунтом не может зайти как клиент");
                    }

                    await Authenticate(login,
                        position ?? "Client", Account.Object.User.Id, passwd);

                    Context.Dispose();

                    return RedirectToAction("Index", "Home");
                }
                catch (Exception exception)
                {
                    this.HandleException(exception);
                }
            }
            return PrivateLogin(Account.Object);
        }
        #endregion

        private async Task Authenticate(string userName, string RoleName, int PersonId, string Password)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, RoleName),
                new Claim(nameof(PersonId), PersonId.ToString()),
                new Claim(nameof(Password), Password),
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            await Context.Database.ExecuteSqlInterpolatedAsync($" call updateexistingroles_r({userName}, {Password});");

            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        #region Logout
        public IActionResult Logout()
        {
            return View("SubmitLogout", new ObjectViewModel<int>
            {
                Action = nameof(Logout),

            });
        }
        [HttpPost]
        [ActionName("Logout")]
        public async Task<IActionResult> LogoutPost()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        #endregion

        #region Registry

        private IActionResult PrivateRegistry(RegistryViewModel Registry = null)
        {
            ViewBag.Positions = Context.Position.Select(p => p.Name);
            return View(
                new ObjectViewModel<RegistryViewModel>
                {
                    Object = Registry ?? new RegistryViewModel
                    {
                        Person = new Person
                        {
                            Staffonly = false,
                            Birthdate = DateTime.Now
                        },
                        Account = new AccountViewModel
                        {
                            Login = ""
                        }
                    },
                    Action = "Registry"
                }
                );
        }
        public IActionResult Registry()
        {
            return PrivateRegistry();
        }
        [HttpPost]
        public async Task<IActionResult> Registry([FromForm] ObjectViewModel<RegistryViewModel> Registry)
        {
            if (ModelState.IsValid)
            {
                using (var trans = Context.Database.BeginTransaction())
                {
                    try
                    {
                        var person = Context.Person.FirstOrDefault(u => u.Email == Registry.Object.Person.Email);
                        if (person == null)
                        {
                            Registry.Object.Account.Login = Registry.Object.Person.Email;
                            Context.Add(Registry.Object.Person);
                            await Context.SaveChangesAsync();

                            await Context.Database.ExecuteSqlRawAsync($"call addnewacc_r('{Registry.Object.Account.Login}','{Registry.Object.Account.Password}');");
                            trans.Commit();
                            return RedirectToAction("Index", "Home");
                        }
                        if (Context.Person.Select(p => Context.CountRoleByName(person.Email) == 0).First() )
                        {
                            throw new Exception("Мы знаем, что вы были клиентом нашего клуба, свяжитесь с нашим менеджером, чтобы восстановить аккаунт");
                        }
                        else
                        {
                            throw new Exception("Некорректные логин и(или) пароль");
                        }
                    }
                    catch (Exception exception)
                    {
                        trans.Rollback();
                        this.HandleException(exception);
                    }
                }
            }
            return PrivateRegistry(Registry.Object);
        }
        #endregion

        #region MyProfile
         private IActionResult PrivateMyProfile(RegistryViewModel MyProfile = null)
        {
            return View(nameof(Registry),
                new ObjectViewModel<RegistryViewModel>
                {
                    Object = MyProfile ?? new RegistryViewModel
                    {
                        Account = new AccountViewModel { 
                            Login = User.Identity.Name,
                            Password = User.Password()
                        },//Context.Account.First(p => p.Id == User.AccountId()),
                        Person = Context.Person.First(p => p.Id == User.PersonId()),
                        IsStaff = Context.Person.Select(a => Context.IsStaff(User.PersonId())).First()
                    },
                    Action = "MyProfile"
                }
                );
        }
        public IActionResult MyProfile()
        {
            return PrivateMyProfile();
        }
        [HttpPost]
        public async Task<IActionResult> MyProfile([FromForm] ObjectViewModel<RegistryViewModel> MyProfile)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = Context.Database.BeginTransaction())
                {
                    try
                    {
                        var Person = await Context.Person.FirstAsync(p => p.Id == MyProfile.Object.Person.Id);
                        Person.Name = MyProfile.Object.Person.Name;
                        Person.Surname = MyProfile.Object.Person.Surname;
                        Person.Email = MyProfile.Object.Person.Email;
                        Person.Phone = MyProfile.Object.Person.Phone;
                        Person.Birthdate = MyProfile.Object.Person.Birthdate;
                        Person.Sex = MyProfile.Object.Person.Sex;
                        Person.Staffonly = MyProfile.Object.Person.Staffonly;

                        await Context.SaveChangesAsync();

                        MyProfile.Object.Account.Login = Person.Email;

                        if(string.IsNullOrEmpty( MyProfile.Object.Account.Password))
                        {
                            MyProfile.Object.Account.Password = User.Password();
                        }
                        else if (User.Identity.Name == MyProfile.Object.Account.Login)
                        {
                            await Context.Database.ExecuteSqlInterpolatedAsync($" call updateexistingroles_r({MyProfile.Object.Account.Login}, {MyProfile.Object.Account.Password});");
                        }

                        if (User.Identity.Name != MyProfile.Object.Account.Login)
                        {
                            string oldLogin = User.Identity.Name;
                            await Context.Database.ExecuteSqlInterpolatedAsync($"call populateallvalidsp_newlogin_r({MyProfile.Object.Account.Login},{User.Identity.Name},{MyProfile.Object.Account.Password});");
                            await Authenticate(MyProfile.Object.Account.Login, User.Role(), Person.Id, MyProfile.Object.Account.Password);
                            await Context.Database.ExecuteSqlInterpolatedAsync($"call removeallexistingroles_r({oldLogin});");
                        }
                        else
                        {
                            await Authenticate(MyProfile.Object.Account.Login, User.Role(), Person.Id, MyProfile.Object.Account.Password);
                        }
                        await transaction.CommitAsync();
                        return RedirectToAction("Index", "Home");
                    }
                    catch (Exception exception)
                    {
                        transaction.Rollback();
                        this.HandleException(exception);
                    }
                }
            }
            return PrivateMyProfile(MyProfile.Object);
        }
        #endregion

        #region SubmitDelete
        public IActionResult SubmitDelete()
        {
            return View(new ObjectViewModel<int>
            {
                Action = nameof(SubmitDelete)
            }
            );
        }

        [HttpPost]
        [ActionName("SubmitDelete")]
        public async Task<IActionResult> SubmitDeletePost()
        {
            Context.DeleteProfile();

            return await LogoutPost();
        }
        #endregion

    }
}
