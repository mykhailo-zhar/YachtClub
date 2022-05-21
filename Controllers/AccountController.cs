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
        private IActionResult PrivateLogin(Account account = null)
        {
            ViewBag.Positions = Context.Position.Select(p => p.Name);
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
        [HttpPost]
        public async Task<IActionResult> Login([FromForm] ObjectViewModel<Account> Account)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var login = Account.Object.Login;
                    switch (Account.Object.AsWho)
                    {
                        case "Staff":
                            if (!Context.Position.Any(p => p.Name == Account.Object.Position))
                                throw new Exception("Выберите, пожалуйста, действительную должность");
                            login += $"Staff_{Account.Object.Position}";
                            break;
                        case "Client": break;
                        default:
                            throw new Exception("Войдите, пожалуйста, либо как сотрудник, либо как клиент");
                    }

                    var passwd = MD5_Extension.HashToString(Account.Object.Password);
                    var user = Context.Account.Include(p => p.User).FirstOrDefault(p => p.Login == login && p.Password == passwd);

                    if (user != null)
                    {
                        if (user.User.Staffonly && Account.Object.AsWho == "Client") {
                            throw new Exception("Человек с служебным аккаунтом не может зайти как клиент");
                        }

                        await Authenticate(user.Login, 
                            Account.Object.Position ?? "");
                        return RedirectToAction("Index", "Home");
                    }
                    ModelState.AddModelError("", "Некорректные логин и(или) пароль");
                }
                catch (Exception exception)
                {
                    this.HandleException(exception);
                }
            }
            return PrivateLogin(Account.Object);
        }

        private async Task Authenticate(string userName, string RoleName)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName),
                new Claim(nameof(RoleName),RoleName)
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return View("SubmitLogout", new ObjectViewModel<int> { 
                Action = nameof(Logout),

            });
        }
        [HttpPost]
        [ActionName("Logout")]
        public IActionResult LogoutPost()
        {
            return RedirectToAction("Index", "Home");
        }

        private IActionResult PrivateRegistry(RegistryViewModel Registry = null)
        {
            ViewBag.Positions = Context.Position.Select(p => p.Name);
            return View(
                new ObjectViewModel<RegistryViewModel>
                {
                    Object = Registry ?? new RegistryViewModel { 
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
                            Password = MD5_Extension.HashToString(Acc.Password) 
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
    }
}
