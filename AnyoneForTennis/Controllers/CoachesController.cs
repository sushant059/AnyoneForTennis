using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AnyoneForTennis.Data;
using AnyoneForTennis.Models;
using AnyoneForTennis.ViewModels;
using Microsoft.AspNetCore.Identity;
using AnyoneForTennis.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;

namespace AnyoneForTennis.Controllers;

[Authorize]
public class CoachesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public CoachesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: Coaches
    [Authorize(Roles = "admin,coach,member")]
    public async Task<IActionResult> Index()
    {
        var query = _context.Coaches.AsQueryable();
        if (User.IsInRole("coach"))
        {
            var id = await GetCoachId();
            query = query.Where(c => c.CoachId == id);
        }
        return View(await query.Include(c => c.User).OrderByDescending(c => c.CoachId).ToListAsync());
    }

    // GET: Coaches/Details/5
    [Authorize(Roles = "admin,coach,member")]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var coach = await _context.Coaches
            .Include(c => c.User)
            .FirstOrDefaultAsync(m => m.CoachId == id);
        if (coach == null)
        {
            return NotFound();
        }

        return View(coach);
    }

    // GET: Coaches/Create
    [Authorize(Roles = "admin")]
    public IActionResult Create()
    {
        return View();
    }

    // POST: Coaches/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [Authorize(Roles = "admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("FirstName,LastName,Biography,Photo,Email,Password")] RegisterCoachViewModel coach)
    {
        if (ModelState.IsValid)
        {
            ApplicationUser newUser = new()
            {
                Email = coach.Email,
                UserName = coach.Email
            };
            IdentityResult result = await _userManager.CreateAsync(newUser,coach.Password);
            if (result.Succeeded)
            {
                Coach newCoach = new()
                {
                    FirstName = coach.FirstName,
                    LastName = coach.LastName,
                    Biography = coach.Biography,
                    Photo = coach.Photo,
                    UserId = newUser.Id,
                };
                _context.Add(newCoach);
                await _context.SaveChangesAsync();
                await _userManager.AddToRoleAsync(newUser, "coach");
                return RedirectToAction(nameof(Index));
            }
            List<string> errors = result.Errors.Select(x => x.Description).ToList();
            string errorMessage = string.Join(", ", errors);
            TempData["IdentityError"] = $"Error : {errorMessage}";
            return View(coach);
        }
        return View(coach);
    }

    // GET: Coaches/Edit/5
    [Authorize(Roles = "admin,coach")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var coach = await _context.Coaches.FindAsync(id);
        if (coach == null)
        {
            return NotFound();
        }
        return View(coach);
    }

    // POST: Coaches/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [Authorize(Roles = "admin,coach")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("CoachId,FirstName,LastName,Biography,Photo,UserId")] Coach coach)
    {
        if (id != coach.CoachId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(coach);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CoachExists(coach.CoachId))
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
        return View(coach);
    }

    [Authorize(Roles = "coach")]
    public async Task<IActionResult> UpdateProfile()
    {
        int id = await GetCoachId();
        if (id == 0)
        { 
            return NotFound(); 
        }
        else
        {
            return RedirectToAction("Edit", new { id });
        }        
    }

    // GET: Coaches/Delete/5
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var coach = await _context.Coaches
            .Include(c => c.User)
            .FirstOrDefaultAsync(m => m.CoachId == id);
        if (coach == null)
        {
            return NotFound();
        }

        return View(coach);
    }

    // POST: Coaches/Delete/5
    [Authorize(Roles = "admin")]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var coach = await _context.Coaches.FindAsync(id);
        if (coach != null)
        {
            _context.Coaches.Remove(coach);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool CoachExists(int id)
    {
        return _context.Coaches.Any(e => e.CoachId == id);
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
