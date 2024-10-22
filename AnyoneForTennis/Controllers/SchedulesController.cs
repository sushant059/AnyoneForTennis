using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AnyoneForTennis.Data;
using AnyoneForTennis.Models;
using Microsoft.AspNetCore.Authorization;
using AnyoneForTennis.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace AnyoneForTennis.Controllers;

[Authorize]
public class SchedulesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public SchedulesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: Schedules
    [Authorize(Roles ="admin,coach,member")]
    public async Task<IActionResult> Index(int? id)
    {
        var query = _context.Schedules.AsQueryable();
        var coachId = await GetCoachId();
        if (User.IsInRole("coach"))
        {
            query = query.Where(s => s.CoachId == coachId);
            if (id == 1)
            {
                query = query.Where(s => s.ScheduledOn >= DateTime.Now);
            }
        }
        return View(await query.Include(x => x.Coach).OrderByDescending(s => s.ScheduledOn).ToListAsync());
    }

    // GET: Schedules/Details/5
    [Authorize(Roles ="admin,coach,member")]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var schedule = await _context.Schedules
            .Include(s => s.Coach)
            .FirstOrDefaultAsync(m => m.ScheduleId == id);
        if (schedule == null)
        {
            return NotFound();
        }

        return View(schedule);
    }

    // GET: Schedules/Create
    [Authorize(Roles = "admin")]
    public IActionResult Create()
    {
        ViewData["CoachId"] = new SelectList(_context.Coaches, "CoachId", "FirstName");
        return View();
    }

    // POST: Schedules/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Create([Bind("ScheduleId,Name,Location,Description,ScheduledOn,CoachId")] Schedule schedule)
    {
        if (ModelState.IsValid)
        {
            _context.Add(schedule);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewData["CoachId"] = new SelectList(_context.Coaches, "CoachId", "FirstName", schedule.CoachId);
        return View(schedule);
    }

    // GET: Schedules/Edit/5
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var schedule = await _context.Schedules.FindAsync(id);
        if (schedule == null)
        {
            return NotFound();
        }
        ViewData["CoachId"] = new SelectList(_context.Coaches, "CoachId", "FirstName", schedule.CoachId);
        return View(schedule);
    }

    // POST: Schedules/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Edit(int id, [Bind("ScheduleId,Name,Location,Description,ScheduledOn,CoachId")] Schedule schedule)
    {
        if (id != schedule.ScheduleId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(schedule);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ScheduleExists(schedule.ScheduleId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        ViewData["CoachId"] = new SelectList(_context.Coaches, "CoachId", "FirstName", schedule.CoachId);
        return View(schedule);
    }

    // GET: Schedules/Delete/5
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var schedule = await _context.Schedules
            .Include(s => s.Coach)
            .FirstOrDefaultAsync(m => m.ScheduleId == id);
        if (schedule == null)
        {
            return NotFound();
        }

        return View(schedule);
    }

    // POST: Schedules/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var schedule = await _context.Schedules.FindAsync(id);
        if (schedule != null)
        {
            _context.Schedules.Remove(schedule);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ScheduleExists(int id)
    {
        return _context.Schedules.Any(e => e.ScheduleId == id);
    }
    private async Task<int> GetCoachId()
    {
        ApplicationUser? usr = await _userManager.GetUserAsync(HttpContext.User);
        if (usr == null)
        {
            return 0;
        }
        int coachId = await _context.Coaches.Where(x => x.UserId == usr.Id).Select(x => x.CoachId).FirstOrDefaultAsync();
        return coachId;
    }
}
