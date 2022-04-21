using Microsoft.AspNetCore.Mvc;
using Project.Database;
using Project.Migrations;

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

        public IActionResult ReloadDatabase()
        {
            SeedData.RestartDatabase(Context);
            SeedData.SeedWithData(Context);
            return View(nameof(Index));
        }
        
        public IActionResult AddValues()
        {
            SeedData.SeedWithData(Context);
            return View(nameof(Index));
        }

        
    }
}
