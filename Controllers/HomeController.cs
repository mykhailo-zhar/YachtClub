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
    public class HomeController : Controller
    {
        public DataContext Context;
        public HomeController(DataContext context)
        {
            Context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = RolesReadonly.DB_Admin)]
        public IActionResult ReloadDatabase()
        {
            SeedData.RestartDatabase(Context);
            SeedData.SeedWithData(Context);
            SeedData.SeedWithProcedure(Context);
            SeedData.SeedAccounts(Context);
            return View(nameof(Index));
        }
    }
}
