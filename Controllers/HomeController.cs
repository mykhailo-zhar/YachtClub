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
                .OrderByDescending(p => p.Startdate)
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
                .Select(p => new MinCrewViewModel
                {
                    Yachttype = p,
                    Yachts = Context.Yacht.Count(a => a.Typeid == p.Id)
                })
               .OrderByDescending(p => p.Yachts)

                ;
            return View(Object);
        }
        public IActionResult Contracttype()
        {
            var Object = Context.Contracttype
               .Select(p => new Contracttype
               {
                   Id = p.Id,
                   Name = p.Name,
                   Price = p.Price,
                   Description = p.Description,
                   Count = Context.CountCTypes(p.Id)
               })
               .OrderByDescending(p => p.Count);
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
                .Select(p => new Yachtleasetype
                {
                    Id = p.Id,
                    Count = Context.CountYLTypes(p.Id),
                    Name = p.Name,
                    Price = p.Price,
                    Description = p.Description
                })
                .OrderByDescending(p => p.Count);
            return View(Object);
        }
    }
}
