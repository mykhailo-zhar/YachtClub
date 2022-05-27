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

        public IActionResult Yachttype()
        {
            var Object = Context.Yachttype
              .OrderBy(p => p.Id)
                .Select(p => new MinCrewViewModel { 
                    Yachttype = p
                })
                ;
            return View(Object);
        }
        public IActionResult Contracttype()
        {
            var Object = Context.Contracttype
               .OrderBy(p => p.Id);
            return View(Object);
        }        
        public IActionResult Yacht()
        {
            var Object = Context.Yacht
                .Include(p => p.Yachtowner)
                .Include(p => p.Type)
                .Select(
                    p => new YachtWithStatusViewModel
                    {
                        Yacht = p,
                        Status = Context.YachtsStatus(p.Id),

                    }
                )
                .Where(p => p.Status == "Готова принимать контракты")
                .ToList()
                ;
            Object.ForEach(p =>
            {
                p.Crew = Context.CaptainByYachtid(p.Yacht.Id)
                   .Include(p => p.Crew)
                       .ThenInclude(p => p.Staff);

            });
            return View(Object);
        }

        public IActionResult Yachtleasetype()
        {
            var Object = Context.Yachtleasetype
                .Where(p => !p.Staffonly)
                .OrderBy(p => p.Id);
            return View(Object);
        }
    }
}
