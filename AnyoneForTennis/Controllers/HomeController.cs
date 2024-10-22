using AnyoneForTennis.Data;
using AnyoneForTennis.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace AnyoneForTennis.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<Schedule> upcommingSchedules = await _context.Schedules.Where(s => s.ScheduledOn >= DateTime.Now)
                                                                        .Include(s => s.Coach)
                                                                        .OrderByDescending(s => s.ScheduledOn)
                                                                        .ToListAsync();
            return View(upcommingSchedules);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
