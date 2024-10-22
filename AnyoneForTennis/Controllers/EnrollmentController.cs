using AnyoneForTennis.Areas.Identity.Data;
using AnyoneForTennis.Data;
using AnyoneForTennis.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AnyoneForTennis.Controllers;

public class EnrollmentController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public EnrollmentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [Authorize(Roles = "member")]
    public async Task<IActionResult> AddEnrollment(int id)
    {
        int memberId = await GetMemberId();
        if(memberId == 0)
        {
            TempData["EnrolledResult"] = $"Cannot Find Member";
            return RedirectToAction("Index", "Home");
        }
        var enrollmentExits = await _context.Enrollments.AnyAsync(x => x.MemberId == memberId && x.ScheduleId == id);
        if (enrollmentExits)
        {
            TempData["EnrolledResult"] = $"Member Already Enrolled in Schedule";
            return RedirectToAction("Index", "Home");
        }
        var schedule = await _context.Schedules.Where(x => x.ScheduleId == id).FirstOrDefaultAsync();
        if(schedule!.ScheduledOn <= DateTime.Now)
        {
            TempData["EnrolledResult"] = $"Emrollment Expired for the Schedule";
            return RedirectToAction("Index", "Home");
        }
        Enrollment newEnrollment = new()
        {
            MemberId = memberId,
            ScheduleId = id
        };
        await _context.Enrollments.AddAsync(newEnrollment);
        await _context.SaveChangesAsync();
        TempData["EnrolledResult"] = $"Member enrolled to Schedule";
        return RedirectToAction("Index", "Home");
    }

    [Authorize(Roles = "admin,coach")]
    public async Task<IActionResult> GetEnrolledMembers(int id)
    {
        List<Enrollment> enrolledMembers = await _context.Enrollments.Where(x => x.ScheduleId == id).Include(x => x.Member).ToListAsync();
        return View(enrolledMembers);
    }

    [Authorize(Roles = "member")]
    public async Task<IActionResult> GetEnrolledSchedules()
    {
        var memberId = await GetMemberId();
        List<Enrollment> enrolledSchedules = await _context.Enrollments.Where(x => x.MemberId == memberId).Include(x => x.Schedule).ToListAsync();
        return View(enrolledSchedules);
    }
    private async Task<int> GetMemberId()
    {
        ApplicationUser? usr = await _userManager.GetUserAsync(HttpContext.User);
        if(usr == null)
        {
            return 0;
        }
        int memberId = await _context.Members.Where(x => x.UserId == usr.Id).Select(x => x.MemberId).FirstOrDefaultAsync();
        return memberId;
    }
}
