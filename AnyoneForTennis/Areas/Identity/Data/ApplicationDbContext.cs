using AnyoneForTennis.Areas.Identity.Data;
using AnyoneForTennis.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AnyoneForTennis.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Schedule> Schedules { get; set; }
    public DbSet<Member> Members { get; set; }
    public DbSet<Coach> Coaches { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);

        builder.Entity<Enrollment>().HasOne(x => x.Schedule).WithMany().HasForeignKey(x => x.ScheduleId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<Enrollment>().HasOne(x => x.Member).WithMany().HasForeignKey(x => x.MemberId).OnDelete(DeleteBehavior.Restrict);

        string ADMIN_ID = Guid.NewGuid().ToString();
        string COACH_ID = Guid.NewGuid().ToString();

        string ADMIN_ROLE_ID = Guid.NewGuid().ToString();
        string COACH_ROLE_ID = Guid.NewGuid().ToString();

        //adding the identity roles
        builder.Entity<IdentityRole>().HasData(new IdentityRole
        {
            Id = ADMIN_ROLE_ID,
            Name = "admin",
            NormalizedName = "ADMIN",
            ConcurrencyStamp = Guid.NewGuid().ToString()
        });
        builder.Entity<IdentityRole>().HasData(new IdentityRole
        {
            Id = COACH_ROLE_ID,
            Name = "coach",
            NormalizedName = "COACH",
            ConcurrencyStamp = Guid.NewGuid().ToString()
        });
        builder.Entity<IdentityRole>().HasData(new IdentityRole
        {
            Id = Guid.NewGuid().ToString(),
            Name = "member",
            NormalizedName = "MEMBER",
            ConcurrencyStamp = Guid.NewGuid().ToString()
        });

        //adding the admin account
        builder.Entity<ApplicationUser>().HasData(new ApplicationUser
        {
            Id = ADMIN_ID,
            UserName = "admin@gmail.com",
            NormalizedUserName = "ADMIN@GMAIL.COM",
            Email = "admin@gmail.com",
            NormalizedEmail = "ADMIN@GMAIL.COM",
            EmailConfirmed = false,
            PasswordHash = "AQAAAAEAACcQAAAAEAiCOqRFc3+nhMuP+sAuf+zLMKxUgQ7S+XggcrDeSGMo9sKnUTAjRcI9UtcWz3X/eA==",
            SecurityStamp = "PYWJWP4GVEGIQZAGDCOBXRVND3E5N2O6",
            ConcurrencyStamp = Guid.NewGuid().ToString()
        });

        //adding the coach Account
        builder.Entity<ApplicationUser>().HasData(new ApplicationUser
        {
            Id = COACH_ID,
            UserName = "coach@gmail.com",
            NormalizedUserName = "COACH@GMAIL.COM",
            Email = "coach@gmail.com",
            NormalizedEmail = "COACH@GMAIL.COM",
            EmailConfirmed = false,
            PasswordHash = "AQAAAAEAACcQAAAAEAiCOqRFc3+nhMuP+sAuf+zLMKxUgQ7S+XggcrDeSGMo9sKnUTAjRcI9UtcWz3X/eA==",
            SecurityStamp = "PYWJWP4GVEGIQZAGDCOBXRVND3E5N2O6",
            ConcurrencyStamp = Guid.NewGuid().ToString()
        });

        //adding the relationship
        List<IdentityUserRole<string>> userRoleMapping =
        [
            new() {
                RoleId = ADMIN_ROLE_ID,
                UserId = ADMIN_ID
            },
            new() {
                RoleId = COACH_ROLE_ID,
                UserId = COACH_ID
            }
        ];
        builder.Entity<IdentityUserRole<string>>().HasData(userRoleMapping);

        //adding COACH
        builder.Entity<Coach>().HasData(new Coach
        {
            FirstName = $"Test",
            LastName =$"Coach",
            CoachId = 1,
            UserId = COACH_ID
        });
    }
}
