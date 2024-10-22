using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AnyoneForTennis.Data;
using AnyoneForTennis.Models;
using AnyoneForTennis.ViewModels;
using AnyoneForTennis.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace AnyoneForTennis.Controllers;

[Authorize]
public class MembersController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public MembersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: Members
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Index()
    {
        var applicationDbContext = _context.Members.Include(m => m.User);
        return View(await applicationDbContext.OrderByDescending(m => m.MemberId).ToListAsync());
    }

    // GET: Members/Details/5
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var member = await _context.Members
            .Include(m => m.User)
            .FirstOrDefaultAsync(m => m.MemberId == id);
        if (member == null)
        {
            return NotFound();
        }

        return View(member);
    }

    // GET: Members/Create
    [Authorize(Roles = "admin")]
    public IActionResult Create()
    {
        return View();
    }

    // POST: Members/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [Authorize(Roles = "admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("FirstName,LastName,Email,Password")] RegisterMemberViewModel member)
    {
        if (ModelState.IsValid)
        {
            ApplicationUser newUser = new()
            {
                Email = member.Email,
                UserName = member.Email
            };
            IdentityResult result = await _userManager.CreateAsync(newUser, member.Password);
            if (result.Succeeded)
            {
                var newMember = new Member
                {
                    FirstName = member.FirstName,
                    LastName = member.LastName,
                    Email = member.Email,
                    Active = true,
                    UserId = newUser.Id
                };
                _context.Add(newMember);
                await _context.SaveChangesAsync();
                await _userManager.AddToRoleAsync(newUser, "member");
                return RedirectToAction(nameof(Index));
            }
            List<string> errors = result.Errors.Select(x => x.Description).ToList();
            string errorMessage = string.Join(", ", errors);
            TempData["IdentityError"] = $"Error : {errorMessage}";
            return View(member);
        }
        return View(member);
    }

    // GET: Members/Edit/5
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var member = await _context.Members.FindAsync(id);
        if (member == null)
        {
            return NotFound();
        }
        return View(member);
    }

    // POST: Members/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [Authorize(Roles = "admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("MemberId,FirstName,LastName,Email,Active,UserId")] Member member)
    {
        if (id != member.MemberId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(member);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MemberExists(member.MemberId))
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
        return View(member);
    }

    // GET: Members/Delete/5
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var member = await _context.Members
            .Include(m => m.User)
            .FirstOrDefaultAsync(m => m.MemberId == id);
        if (member == null)
        {
            return NotFound();
        }

        return View(member);
    }

    // POST: Members/Delete/5
    [Authorize(Roles = "admin")]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var member = await _context.Members.FindAsync(id);
        if (member != null)
        {
            _context.Members.Remove(member);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool MemberExists(int id)
    {
        return _context.Members.Any(e => e.MemberId == id);
    }
}
