using AnyoneForTennis.Data;
using AnyoneForTennis.Models;
using AnyoneForTennis.OldDatas.Data;
using Microsoft.EntityFrameworkCore;

namespace AnyoneForTennis.Areas.Identity.Data;

public class DataInitializer
{
    private readonly ApplicationDbContext _context; 
    private readonly Hitdb1Context _hitdb1Context;
    private readonly ILogger<DataInitializer> _logger;

    public DataInitializer(ApplicationDbContext context,
                           Hitdb1Context hitdb1Context,
                           ILogger<DataInitializer> logger)
    {
        _context = context;
        _hitdb1Context = hitdb1Context;
        _logger = logger;
    }

    public void Run()
    {
        _context.Database.Migrate();
        if (_context.Coaches.Count() < 2 && !_context.Members.Any() && !_context.Schedules.Any())
        {
            MigrateDataFromPreviousDatabase();
        }
    }
    private void MigrateDataFromPreviousDatabase()
    {
        try
        {
            _logger.LogInformation("Database to database migration Started");
            List<OldDatas.Models.Member> oldMembers = [.. _hitdb1Context.Members];
            List<Member> members = oldMembers.Select(x => new Member()
            {
                Email = x.Email ??= "test@mail.com",
                FirstName = x.FirstName ??= "First Name",
                LastName = x.LastName ??= "LastName",
                Active = x.Active
            }).ToList();
            if (members.Count > 0)
            {
                _context.Members.AddRange(members);
                _context.SaveChanges();
            }

            List<OldDatas.Models.Schedule> oldSchedules = [.. _hitdb1Context.Schedules];
            Random rand = new();
            List<Schedule> schedules = oldSchedules.Select(x => new Schedule()
            {
                Name = x.Name,
                Location = x.Location ??= "Location",
                Description = x.Description,
                ScheduledOn = DateTime.Now - TimeSpan.FromDays(rand.Next(0, 30)) - TimeSpan.FromHours(rand.Next(0, 24)) - TimeSpan.FromMinutes(rand.Next(0, 60)),
                CoachId = 1,
            }).ToList();

            if (schedules.Count > 0)
            {
                _context.Schedules.AddRange(schedules);
                _context.SaveChanges();
            }

            List<OldDatas.Models.Coach> oldCoaches = [.. _hitdb1Context.Coaches];
            List<Coach> coaches = oldCoaches.Select(x => new Coach()
            {
                FirstName = x.FirstName,
                LastName = x.LastName,
                Biography = x.Biography,
                Photo = x.Photo,
            }).ToList();
            if (coaches.Count > 0)
            {
                _context.Coaches.AddRange(coaches);
                _context.SaveChanges();
            }
            _logger.LogInformation("Database to Databse migration Successful");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception Thrown while performing database to database migration");
        }
    }
}
