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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

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
            var Object = Context.Event
                .OrderBy(p => p.Startdate)
                .Take(5)
                .ToList()
                .Select(a => new EventWinnerCrewViewModel
                {
                    Event = a,
                    Winners = Context.YachtCrewByEvent(a.Id)
                        .Include(p => p.Crew)
                            .ThenInclude(p => p.Staff)
                        .Include(p => p.Yacht)
                            .ThenInclude(p => p.Type)
                        .ToList()
                        .Select(p => new YachtCrewWinners
                        {
                            Crew = p,
                            Winner = Context.Winner.FirstOrDefault(z => z.Eventid == a.Id && p.Yachtid == z.Yachtid)
                        })
                        .OrderBy(p => p.Winner.Place)
                    ,
                   
                        
                })
                ;

            return View(Object);
        }

        //[Authorize(Roles = RolesReadonly.DB_Admin)]
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
