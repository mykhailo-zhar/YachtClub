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

        private IActionResult PrivateLogin(Account account = null)
        {
            return View(
                new ObjectViewModel<Account>
                {
                    Object = account ?? new Account(),
                    Action = "Login"
                }
                );
        }
        public IActionResult Login()
        {
            return PrivateLogin();
        }

        private IActionResult LoginToRolePrivate(ObjectViewModel<Account> Account)
        {
            Account.Action = nameof(LoginToRole);
            return View("LoginToRole", Account);
        }
        [HttpPost]
        public async Task<IActionResult> LoginToRole(ObjectViewModel<Account> Account)
        {
            Account.Object.AsWho = "Staff";
            return await Login(Account);
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromForm] ObjectViewModel<Account> Account)
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
                    var passwd = Hash_Extension.HashToString(Account.Object.Password);
                    var user = Context.Account.Include(p => p.User).FirstOrDefault(p => p.Login == login && p.Password == passwd);

                    if (user == null)
                    {
                        throw new Exception("Некорректные логин и(или) пароль");
                    }

                    if (Account.Object.Position == null && Account.Object.AsWho == "Staff")
                    {
                        var collection = Context.StaffPosition
                            .Include(p => p.Position)
                            .Where(p => p.Staffid == user.Userid && p.Enddate == null)
                            .Select(p => p.Position.Name)
                            .ToList();
                        if(collection.Count() == 0)
                        {
                            throw new Exception("Вы не можете войти как сотрудник");
                        }

                        ViewBag.Positions = collection;
                        
                        Account.Object.Userid = user.Id;
                        return LoginToRolePrivate(Account);
                    }

                    if (user.User.Staffonly && Account.Object.AsWho == "Client")
                    {
                        throw new Exception("Человек с служебным аккаунтом не может зайти как клиент");
                    }

                    await Authenticate(user.Login,
                        Account.Object.Position ?? "Client", user.Userid, user.Id);

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
        private async Task Authenticate(string userName, string RoleName, int PersonId, int AccountId)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, RoleName),
                new Claim(nameof(PersonId), PersonId.ToString()),
                new Claim(nameof(AccountId), AccountId.ToString())
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

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
                        Account = new Account
                        {
                            Login = "__-2132123113_"
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
                try
                {
                    var person = Context.Person.FirstOrDefault(u => u.Email == Registry.Object.Person.Email);
                    if (person == null)
                    {
                        Context.Add(Registry.Object.Person);
                        await Context.SaveChangesAsync();
                        person = Context.Person.FirstOrDefault(u => u.Email == Registry.Object.Person.Email);
                        var Acc = Registry.Object.Account;
                        var Hash = MD5.Create();
                        Acc = new Account
                        {
                            Login = person.Email,
                            Userid = person.Id,
                            Password = Hash_Extension.HashToString(Acc.Password)
                        };
                        Context.Account.Add(Acc);
                        await Context.SaveChangesAsync();
                        return RedirectToAction("Index", "Home");
                    }
                    ModelState.AddModelError("", "Некорректные логин и(или) пароль");
                }
                catch (Exception exception)
                {
                    this.HandleException(exception);
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
                        Account = Context.Account.First(p => p.Id == User.AccountId()),
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
                        var Acc = await Context.Account.FirstAsync(p => p.Id == MyProfile.Object.Account.Id);

                        if(!string.IsNullOrEmpty(MyProfile.Object.Account.Password))
                        {
                            Acc.Password = Hash_Extension.HashToString(MyProfile.Object.Account.Password);
                        }
                        Acc.Login = Person.Email;
                        await Context.SaveChangesAsync();

                        await Authenticate(Acc.Login, User.Role() , Person.Id, Acc.Id);

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
    }
}
